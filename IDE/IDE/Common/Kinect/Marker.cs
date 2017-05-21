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
    }
}
