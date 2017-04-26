using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IDE.Common.ViewModels.Converters
{
    public class ColorToBrush : IValueConverter
    {
        private static readonly Color DEFAULT_COLOR = Colors.White;
        private static readonly Brush DEFAULT_BRUSH = Brushes.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                var color = (Color) value;
                return new SolidColorBrush(color);
            }
            return DEFAULT_BRUSH;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush)
            {
                var brush = (SolidColorBrush)value;
                return brush.Color;
            }
            return DEFAULT_COLOR;
        }
    }
}
