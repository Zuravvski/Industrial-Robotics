using IDE.Common.ViewModels.Commands;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = System.Drawing.Point;

namespace IDE.Common.Kinect
{
    public class KinectHandler : ObservableObject
    {

        #region Fields
        
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

        private bool seatedModeIsChecked;
        private string connectionStatusText;
        private string trackingStatusText;
        private string lHandText;
        private string rHandText;

        #endregion

        public KinectHandler()
        {
            lastPosition = new SkeletonPoint()
            {
                X = 0,
                Y = 0,
                Z = 0
            };
            InitializeKinect();
        }

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
            kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            kinectSensor.SkeletonStream.Enable(smoothingParam);
            kinectSensor.DepthStream.Enable();
            kinectSensor.ColorStream.Enable();

            interactionStream = new InteractionStream(kinectSensor, new InteractionClient());
            interactionStream.InteractionFrameReady += InteractionStream_InteractionFrameReady;

            kinectSensor.AllFramesReady += KinectSensor_AllFramesReady;

            kinectSensor.Start();
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            ConnectionStatusText = kinectSensor?.Status.ToString();
            if (kinectSensor?.Status == KinectStatus.Connected)
                InitializeKinect();
        }

        private async void InteractionStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            using (InteractionFrame interactionFrame = e.OpenInteractionFrame()) //dispose as soon as possible
            {
                if (interactionFrame == null)
                    return;

                interactionFrame.CopyInteractionDataTo(userInfos);
            }

            foreach (var userInfo in userInfos)
            {
                var userID = userInfo.SkeletonTrackingId;
                if (userID == 0)
                    continue;


                hands = userInfo.HandPointers;

                if (hands.Count > 0)
                {
                    var mySkeleton = skeletons.FirstOrDefault(skeleton => skeleton.TrackingState == SkeletonTrackingState.Tracked);

                    var leftHand = hands.FirstOrDefault(hand => hand.HandType == InteractionHandType.Left);
                    var rightHand = hands.FirstOrDefault(hand => hand.HandType == InteractionHandType.Right);

                    if (leftHand == null || rightHand == null)
                        return;

                    var leftHandEvent = HandleHandEvent(leftHand, userID);
                    var rightHandEvent = HandleHandEvent(rightHand, userID);

                    LHandText = $"Tracking status: {leftHand.IsTracked}\nX: {Math.Round(leftHand.RawX, 1)} Y: {Math.Round(leftHand.RawY, 1)} Z: {Math.Round(leftHand.RawZ, 1)}\nState: {leftHandEvent}";
                    var leftHandJoint = mySkeleton.Joints[JointType.HandLeft];
                    var leftImagePoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(leftHandJoint.Position,
                        DepthImageFormat.Resolution640x480Fps30);

                    LeftHandMarker = new Marker()
                    {
                        CanvasLeft = leftImagePoint.X,
                        CanvasTop = leftImagePoint.Y,
                        Visibility = TrackingToVisibility(leftHandJoint, leftHandEvent),
                        Color = StateToColor(leftHandEvent)
                    };


                    RHandText = $"Tracking status: {rightHand.IsTracked}\nX: {Math.Round(rightHand.RawX, 1)} Y: {Math.Round(rightHand.RawY, 1)} Z: {Math.Round(rightHand.RawZ, 1)}\nState: {rightHandEvent}";
                    var rightHandJoint = mySkeleton.Joints[JointType.HandRight];
                    var rightImagePoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(rightHandJoint.Position,
                        DepthImageFormat.Resolution640x480Fps30);

                    RightHandMarker = new Marker()
                    {
                        CanvasLeft = rightImagePoint.X,
                        CanvasTop = rightImagePoint.Y,
                        Visibility = TrackingToVisibility(rightHandJoint, leftHandEvent),
                        Color = StateToColor(rightHandEvent)
                    };

                    var xAddition = 0;
                    var yAddition = 0;
                    var zAddition = 0;
                    var multiplication = 100;    //tbd

                    var deltaX = rightHandJoint.Position.X - lastPosition.X;
                    var deltaY = rightHandJoint.Position.Y - lastPosition.Y;
                    var deltaZ = rightHandJoint.Position.Z - lastPosition.Z;

                    if (Math.Abs(deltaX) > 0.1)
                    {
                        xAddition = (int)deltaX * multiplication;
                    }
                    if (Math.Abs(deltaY) > 0.1)
                    {
                        yAddition = (int)deltaY * multiplication;
                    }
                    if (Math.Abs(deltaZ) > 0.1)
                    {
                        zAddition = (int)deltaY * multiplication;
                    }

                    if (leftHandEvent == InteractionHandEventType.Grip)
                    {
                        if (rightHandEvent == InteractionHandEventType.Grip)
                        {
                            Debug.WriteLine("GC");
                        }
                        else if (rightHandEvent == InteractionHandEventType.GripRelease)
                        {
                            Debug.WriteLine("GO");
                        }
                        //delay
                        await Task.Delay(250);
                        Debug.WriteLine($"DS {zAddition}, {xAddition}, {yAddition}");
                        //DS zAddition, xAddition, yAddition
                    }
                    else
                    {
                        lastPosition = new SkeletonPoint() { X = 0, Y = 0, Z = 0 };
                    }



                    lastPosition = rightHandJoint.Position;
                }
            }
        }

        private InteractionHandEventType HandleHandEvent(InteractionHandPointer hand, int userID)
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


        private Visibility TrackingToVisibility(Joint hand, InteractionHandEventType leftHandEvent)
        {
            if (hand.JointType == JointType.HandLeft)
            {
                if (hand.TrackingState == JointTrackingState.Tracked)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
            else
            {
                if (hand.TrackingState == JointTrackingState.Tracked && leftHandEvent == InteractionHandEventType.Grip)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        private SolidColorBrush StateToColor (InteractionHandEventType state)
        {
            if (state == InteractionHandEventType.Grip)
                return new SolidColorBrush(Colors.Green);
            else
                return new SolidColorBrush(Colors.Red);
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
                catch (InvalidOperationException)
                {
                    // DepthFrame functions may throw when the sensor gets
                    // into a bad state.  Ignore the frame in that case.
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
                catch (InvalidOperationException)
                {
                    // SkeletonFrame functions may throw when the sensor gets
                    // into a bad state.  Ignore the frame in that case.
                }
            }
        }

        #region Properties

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

    }
}
