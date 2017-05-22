using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Windows;
using System.Windows.Media;

namespace IDE.Common.Kinect
{
    public class Marker
    {
        public int CanvasLeft { get; set; }
        public int CanvasTop { get; set; }
        public SolidColorBrush Color { get; set; }
        public Visibility Visibility { get; set; }

        public void UpdateUI(KinectSensor kinectSensor, Joint handJoint, InteractionHandEventType handEvent, InteractionHandEventType handColor)
        {
            //calculate values
            var leftImagePoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(handJoint.Position,
                    DepthImageFormat.Resolution640x480Fps30);

            //update UI
            CanvasLeft = leftImagePoint.X;
            CanvasTop = leftImagePoint.Y;
            Visibility = TrackingToVisibility(handJoint, handEvent);
            Color = StateToColor(handColor);
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

        private SolidColorBrush StateToColor(InteractionHandEventType state)
        {
            if (state == InteractionHandEventType.Grip)
                return new SolidColorBrush(Colors.Green);
            else
                return new SolidColorBrush(Colors.Red);
        }
    }
}