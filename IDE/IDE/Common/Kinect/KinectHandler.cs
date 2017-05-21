using IDE.Common.ViewModels.Commands;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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


        private bool seatedModeIsChecked;
        private string connectionStatusText;
        private string trackingStatusText;
        private string lHandText;
        private string rHandText;

        #endregion

        public KinectHandler()
        {
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

        private void InteractionStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
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

                if (hands.Count != 0)
                {
                    foreach (var hand in hands)
                    {
                        var lastHandEvents = hand.HandType == InteractionHandType.Left
                                                    ? _lastLeftHandEvents
                                                    : _lastRightHandEvents;

                        if (hand.HandEventType != InteractionHandEventType.None)
                            lastHandEvents[userID] = hand.HandEventType;

                        var lastHandEvent = lastHandEvents.ContainsKey(userID)
                                                ? lastHandEvents[userID]
                                                : InteractionHandEventType.None;


                        var mySkeleton =  skeletons.FirstOrDefault(skeleton => skeleton.TrackingState == SkeletonTrackingState.Tracked);

                        if (hand.HandType == InteractionHandType.Left)
                        {
                            LHandText = $"Tracking status: {hand.IsTracked}\nX: {Math.Round(hand.RawX, 1)} Y: {Math.Round(hand.RawY, 1)} Z: {Math.Round(hand.RawZ, 1)}\nState: {lastHandEvent}";
                            var leftHand = mySkeleton.Joints[JointType.HandLeft];
                            var imagePoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(leftHand.Position,
                                DepthImageFormat.Resolution640x480Fps30);
                            LeftHandMarker = new Marker()
                            {
                                CanvasLeft = imagePoint.X,
                                CanvasTop = imagePoint.Y,
                                Visibility = TrackingToVisibility(leftHand.TrackingState),
                                Color = StateToColor(lastHandEvent)
                            };
                        }

                        if (hand.HandType == InteractionHandType.Right)
                        {
                            RHandText = $"Tracking status: {hand.IsTracked}\nX: {Math.Round(hand.RawX, 1)} Y: {Math.Round(hand.RawY, 1)} Z: {Math.Round(hand.RawZ, 1)}\nState: {lastHandEvent}";
                            var rightHand = mySkeleton.Joints[JointType.HandRight];
                            var imagePoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(rightHand.Position,
                                DepthImageFormat.Resolution640x480Fps30);
                            RightHandMarker = new Marker()
                            {
                                CanvasLeft = imagePoint.X,
                                CanvasTop = imagePoint.Y,
                                Visibility = TrackingToVisibility(rightHand.TrackingState),
                                Color = StateToColor(lastHandEvent)
                            };
                        }
                    }
                }
            }
        }

        private Visibility TrackingToVisibility(JointTrackingState trackingState)
        {
            if (trackingState == JointTrackingState.Tracked)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
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
