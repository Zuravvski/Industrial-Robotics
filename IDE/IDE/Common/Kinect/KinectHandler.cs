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
        private InteractionHandEventType leftHandLastEvent;
        private InteractionHandEventType rightHandLastEvent;

        private Dictionary<int, InteractionHandEventType> _lastLeftHandEvents = new Dictionary<int, InteractionHandEventType>();
        private Dictionary<int, InteractionHandEventType> _lastRightHandEvents = new Dictionary<int, InteractionHandEventType>();
        private ImageSource imageSource;

        private Ellipse leftHandMarker, rightHandMarker;


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
                smoothingParam.Smoothing = 0.5f;
                smoothingParam.Correction = 0.3f;
                smoothingParam.Prediction = 1f;
                smoothingParam.JitterRadius = 1.0f;
                smoothingParam.MaxDeviationRadius = 1.0f;
            };
            kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            kinectSensor.SkeletonStream.Enable();
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
                            var leftWrist = mySkeleton.Joints[JointType.WristLeft];
                            var leftHand = mySkeleton.Joints[JointType.HandLeft];
                            var wristColorImagePoint = SkeletonPointToColorPoint(mySkeleton, leftWrist);
                            var handColorImagePoint = SkeletonPointToColorPoint(mySkeleton, leftHand);
                            var colorpoints = GetExternedLine(wristColorImagePoint, handColorImagePoint);
                            var length = GetLength(colorpoints[0].X, colorpoints[0].Y, colorpoints[1].X, colorpoints[1].Y);
                            var leftHandPoint = new Point
                            {
                                Y = (int) (colorpoints[0].Y + colorpoints[1].Y) / 2 - length / 2,
                                X = (int) (colorpoints[0].X + colorpoints[1].X) / 2 - length / 2
                            };
                            LeftHandMarker = new Ellipse()
                            {
                                Margin = new Thickness(leftHandPoint.X - 320, leftHandPoint.Y - 240, 0, 0),
                                Width = length,
                                Height = length
                            };
                        }

                        if (hand.HandType == InteractionHandType.Right)
                        {

                            RHandText = $"Tracking status: {hand.IsTracked}\nX: {Math.Round(hand.RawX, 1)} Y: {Math.Round(hand.RawY, 1)} Z: {Math.Round(hand.RawZ, 1)}\nState: {lastHandEvent}";
                            var rightWrist = mySkeleton.Joints[JointType.WristRight];
                            var rightHand = mySkeleton.Joints[JointType.HandRight];
                            var imagePoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(rightHand.Position,
                                DepthImageFormat.Resolution640x480Fps30);
                            RightHandMarker = new Ellipse() {Margin = new Thickness(imagePoint.X - 10, imagePoint.Y - 10, 0, 0)};
                        }
                    }
                }
            }
        }

        private ColorImagePoint[] GetExternedLine(ColorImagePoint p1, ColorImagePoint p2)
        {
            var colorPoints = new ColorImagePoint[2];

            colorPoints[0] = ConvertColorImagePointToPoint(GetLineDoubled(ConvertPointToColorImagePoint(p2), ConvertPointToColorImagePoint(p1)));
            colorPoints[1] = p1;
            return colorPoints;

        }

        private Point ConvertPointToColorImagePoint(ColorImagePoint p1)
        {
            var color2D = new Point
            {
                X = (int) p1.X,
                Y = (int) p1.Y
            };


            return color2D;

        }

        private ColorImagePoint ConvertColorImagePointToPoint(Point p1)
        {
            ColorImagePoint color2D = new ColorImagePoint();
            color2D.X = (int)p1.X;
            color2D.Y = (int)p1.Y;


            return color2D;
        }

        private Point GetLineDoubled(Point midPoints, Point p1)
        {
            if (midPoints == null || p1 == null)
            {
                return midPoints;
            }
            float p2x = (float)(2 * midPoints.X - p1.X);
            float p2y = (float)(2 * midPoints.Y - p1.Y);


            if (p2x < 0)
                p2x = 0.0f;

            if (p2y < 0)
                p2y = 0.0f;



            midPoints.X = (int)p2x;
            midPoints.Y = (int)p2y;



            return midPoints;
        }

        private ColorImagePoint SkeletonPointToColorPoint(Skeleton skeleton, Joint jointValue)
        {

            SkeletonPoint sPoint = new SkeletonPoint
            {
                X = jointValue.Position.X,
                Y = jointValue.Position.Y,
                Z = jointValue.Position.Z
            };

            var colorPoint = kinectSensor.MapSkeletonPointToColor(sPoint, ColorImageFormat.RgbResolution640x480Fps30);

            return colorPoint;
        }

        private int GetLength(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
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

        public Ellipse LeftHandMarker
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

        public Ellipse RightHandMarker
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
