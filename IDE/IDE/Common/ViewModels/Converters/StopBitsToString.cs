using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    [ValueConversion(typeof(StopBits), typeof(string))]
    public class StopBitsToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stopBits = value.ToString();

            if (stopBits == "One")
                return "1";
            if (stopBits == "OnePointFive")
                return "1.5";
            else
                return "2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stopBits = value.ToString();

            if (stopBits == "1")
                return StopBits.One;
            if (stopBits == "1.5")
                return StopBits.OnePointFive;
            else
                return StopBits.Two;
        }
    }
}
