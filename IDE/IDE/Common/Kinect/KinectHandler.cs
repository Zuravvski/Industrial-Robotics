using Driver;
using IDE.Common.ViewModels.Commands;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IDE.Common.Kinect
{
    public class KinectHandler : ObservableObject
    {

        #region Fields

        private Thread consumerThread;
        private ConcurrentQueue<string> bufferedCommands;
        
        private KinectSensor kinectSensor;
        private InteractionStream interactionStream;

        private Skeleton[] skeletons; //the skeletons 
        private UserInfo[] userInfos; //the information about the interactive users
        private IReadOnlyCollection<InteractionHandPointer> hands;

        private Dictionary<int, InteractionHandEventType> _lastLeftHandEvents = new Dictionary<int, InteractionHandEventType>();
        private Dictionary<int, InteractionHandEventType> _lastRightHandEvents = new Dictionary<int, InteractionHandEventType>();
        private ImageSource imageSource;

        private Marker leftHandMarker, rightHandMarker;
        private SkeletonPoint lastPosition;

        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

        private bool seatedModeIsChecked;
        private string connectionStatusText;
        private string trackingStatusText;
        private string lHandText;
        private string rHandText;

        private volatile float sumX, sumY, sumZ;

        #endregion

        #region Constructor

        public KinectHandler()
        {
            lastPosition = new SkeletonPoint() { X = 0, Y = 0, Z = 0 };
            LeftHandMarker = new Marker();
            RightHandMarker = new Marker();
            bufferedCommands = new ConcurrentQueue<string>();
            consumerThread = new Thread(CommandConsumer) { IsBackground = true } ;
            InitializeKinect();

            sumX = 0;
            sumY = 0;
            sumZ = 0;

            t.Interval = 200; // specify interval time as you want
            t.Tick += new EventHandler(timer_Tick);
            t.Start();
            //consumerThread.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            canSend = !canSend;
        }

        #endregion

        #region Properties

        public E3JManipulator Manipulator { get; set; }

        public KinectSensorChooserUI KinectSensorChooserUI { get; set; }

        public Marker LeftHandMarker
        {
            get
            {
                return leftHandMarker;
            }
            set
            {
                leftHandMarker = value;
                NotifyPropertyChanged("LeftHandMarker");
            }
        }

        public Marker RightHandMarker
        {
            get
            {
                return rightHandMarker;
            }
            set
            {
                rightHandMarker = value;
                NotifyPropertyChanged("RightHandMarker");
            }
        }

        public KinectSensor KinectSensor
        {
            get
            {
                return kinectSensor;
            }
            set
            {
                kinectSensor = value;
                NotifyPropertyChanged("KinectSensor");
            }
        }

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                NotifyPropertyChanged("ImageSource");
            }
        }

        public string ConnectionStatusText
        {
            get { return connectionStatusText; }
            set
            {
                connectionStatusText = value;
                NotifyPropertyChanged("ConnectionStatusText");
            }
        }

        public string TrackingStatusText
        {
            get { return trackingStatusText; }
            set
            {
                trackingStatusText = value;
                NotifyPropertyChanged("TrackingStatusText");
            }
        }

        public string LHandText
        {
            get { return lHandText; }
            set
            {
                lHandText = value;
                NotifyPropertyChanged("LHandText");
            }
        }

        public string RHandText
        {
            get { return rHandText; }
            set
            {
                rHandText = value;
                NotifyPropertyChanged("RHandText");
            }
        }

        public bool SeatedModeIsChecked
        {
            get { return seatedModeIsChecked; }
            set
            {
                seatedModeIsChecked = value;
                if (kinectSensor != null)
                {
                    if (seatedModeIsChecked)
                        kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    else
                        kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
                NotifyPropertyChanged("SeatedModeIsChecked");
            }
        }

        #endregion

        #region Actions

        private void InitializeKinect()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No kinect detected!");
                return;
            }

            KinectSensor = KinectSensor.KinectSensors[0];
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            skeletons = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];
            userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];


            var smoothingParam = new TransformSmoothParameters();
            {
                smoothingParam.Smoothing = 0.3f;
                //smoothingParam.Correction = 0.3f;
                //smoothingParam.Prediction = 1f;
                //smoothingParam.JitterRadius = 1.0f;
                //smoothingParam.MaxDeviationRadius = 1.0f;
            };
            kinectSensor.SkeletonStream.Enable(smoothingParam);
            kinectSensor.DepthStream.Enable();
            kinectSensor.ColorStream.Enable();

            interactionStream = new InteractionStream(kinectSensor, new InteractionClient());
            interactionStream.InteractionFrameReady += InteractionStream_InteractionFrameReady;

            kinectSensor.AllFramesReady += KinectSensor_AllFramesReady;

            kinectSensor.Start();
        }

        volatile bool canSend;
        private void CommandConsumer()
        {
            var lastGripCommand = string.Empty;
            var lastMoveCommand = string.Empty;
            while (true)
            {
                //string command;
                //if (bufferedCommands.TryDequeue(out command))
                //{
                //    if (command.Contains("DS"))
                //    {
                //        Manipulator?.SendCustom(command);
                //        Debug.WriteLine(command);
                //        sumX = 0;
                //        sumY = 0;
                //        sumZ = 0;
                //    }
                //    //if (command.Contains("G"))
                //    //{
                //    //    if (!lastGripCommand.Equals(command))
                //    //    {
                //    //        Manipulator?.SendCustom(command);
                //    //        Thread.Sleep(100);
                //    //        Debug.WriteLine(command);
                //    //    }
                //    //    lastGripCommand = command;

                //    //}
                //}
                //Manipulator?.SendCustom($"DS {sumZ}, {sumX}, {sumY}");
                //Debug.WriteLine($"DS {sumZ}, {sumX}, {sumY}");
                //Thread.Sleep(1000);
                //sumX = 0;
                //sumY = 0;
                //sumZ = 0;
                
            }
        }

        private async void HandleSomething()
        {
            Manipulator?.SendCustom($"DS {sumZ}, {sumX}, {sumY}");
            Debug.WriteLine($"DS {sumZ}, {sumX}, {sumY}");
            await Task.Delay(250);
            sumX = 0;
            sumY = 0;
            sumZ = 0;
        }

        private InteractionHandEventType DetermineLastHandEvent(InteractionHandPointer hand, int userID)
        {
            var lastHandEvents = hand.HandType == InteractionHandType.Left
                                                    ? _lastLeftHandEvents
                                                    : _lastRightHandEvents;

            if (hand.HandEventType != InteractionHandEventType.None)
                lastHandEvents[userID] = hand.HandEventType;

            var lastHandEvent = lastHandEvents.ContainsKey(userID)
                                    ? lastHandEvents[userID]
                                    : InteractionHandEventType.None;

            return lastHandEvent;
        }

        #endregion

        #region Events

        private async void InteractionStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            using (InteractionFrame interactionFrame = e.OpenInteractionFrame()) //dispose as soon as possible
            {
                if (interactionFrame == null)
                    return;

                interactionFrame.CopyInteractionDataTo(userInfos);
            }

            //select our user and get his data
            var userInfo = userInfos.FirstOrDefault(uInfo => uInfo.SkeletonTrackingId != 0);
            if (userInfo == null)
                return;

            var userID = userInfo.SkeletonTrackingId;
            hands = userInfo.HandPointers;

            //if at least one hand has been found
            if (hands.Count > 0)
            {
                var mySkeleton = skeletons.FirstOrDefault(skeleton => skeleton.TrackingState == SkeletonTrackingState.Tracked);

                var leftHand = hands.FirstOrDefault(hand => hand.HandType == InteractionHandType.Left);
                var rightHand = hands.FirstOrDefault(hand => hand.HandType == InteractionHandType.Right);

                if (leftHand == null || rightHand == null)
                    return;

                var leftHandEvent = DetermineLastHandEvent(leftHand, userID);
                var rightHandEvent = DetermineLastHandEvent(rightHand, userID);

                var leftHandJoint = mySkeleton.Joints[JointType.HandLeft];
                var rightHandJoint = mySkeleton.Joints[JointType.HandRight];

                LHandText = $"Tracking status: {leftHand.IsTracked}\nX: {Math.Round(leftHand.RawX, 1)} Y: {Math.Round(leftHand.RawY, 1)} Z: {Math.Round(leftHand.RawZ, 1)}\nState: {leftHandEvent}";
                RHandText = $"Tracking status: {rightHand.IsTracked}\nX: {Math.Round(rightHand.RawX, 1)} Y: {Math.Round(rightHand.RawY, 1)} Z: {Math.Round(rightHand.RawZ, 1)}\nState: {rightHandEvent}";

                LeftHandMarker.UpdateUI(kinectSensor, leftHandJoint, leftHandEvent, leftHandEvent);
                NotifyPropertyChanged("LeftHandMarker");
                RightHandMarker.UpdateUI(kinectSensor, rightHandJoint, leftHandEvent, rightHandEvent);
                NotifyPropertyChanged("RightHandMarker");

                int xAddition = 0, yAddition = 0, zAddition = 0;
                var multiplication = 100;    //tbd

                var deltaX = (rightHandJoint.Position.X - lastPosition.X) * multiplication;
                var deltaY = (rightHandJoint.Position.Y - lastPosition.Y) * multiplication;
                var deltaZ = (rightHandJoint.Position.Z - lastPosition.Z) * multiplication;



                
                
                //var moveCommand = $"DS {sumZ}, {sumX}, {sumY}";
                //if (!bufferedCommands.Contains(moveCommand))
                //{
                //    bufferedCommands.Enqueue(moveCommand);
                //}


                //if (Manipulator == null || !Manipulator.Connected)
                //    return;

                if (leftHandEvent == InteractionHandEventType.Grip)
                {
                    if (rightHandEvent == InteractionHandEventType.Grip)
                    {
                        if(!bufferedCommands.Contains("GC"))
                            bufferedCommands.Enqueue("GC");
                    }
                    else if (rightHandEvent == InteractionHandEventType.GripRelease)
                    {
                        if (!bufferedCommands.Contains("GO"))
                            bufferedCommands.Enqueue("GO");
                    }
                    sumX += deltaX;
                    sumY += deltaY;
                    sumZ += deltaZ;
                    
                    lastPosition = rightHandJoint.Position;

                    if (canSend)
                    {
                        if (sumX < 20 && sumY < 20 && sumZ < 20)
                        {
                            Debug.WriteLine($"DS {(int)sumZ}, {(int)sumX}, {(int)sumY}");
                            Manipulator?.SendCustom($"DS {(int)sumZ}, {(int)sumX}, {(int)sumY}");
                        }

                        sumX = 0;
                        sumY = 0;
                        sumZ = 0;
                        canSend = !canSend;
                    }
                }
                else
                {
                    //sumX = 0;
                    //sumY = 0;
                    //sumZ = 0;
                    lastPosition = new SkeletonPoint() { X = 0, Y = 0, Z = 0 };
                }
            }
        }

        private void KinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (var colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                    return;

                var pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                var stride = colorFrame.Width * 4;
                ImageSource = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }

            using (var depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                    return;

                try
                {
                    interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                }
                catch (InvalidOperationException ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }

            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                try
                {
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    var accelerometerReading = kinectSensor.AccelerometerGetCurrentReading();
                    interactionStream.ProcessSkeleton(skeletons, accelerometerReading, skeletonFrame.Timestamp);
                }
                catch (InvalidOperationException ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            ConnectionStatusText = kinectSensor?.Status.ToString();
            if (kinectSensor?.Status == KinectStatus.Connected)
                InitializeKinect();
        }

        #endregion
        
    }
}
