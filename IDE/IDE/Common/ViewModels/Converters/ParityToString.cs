using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    [ValueConversion(typeof(Parity), typeof(string))]
    public class ParityToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parity = (Parity)value;
            return parity.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parityType = value as string;

            if (parityType == "Odd")
                return Parity.Odd;
            if (parityType == "Even")
                return Parity.Even;
            if (parityType == "Mark")
                return Parity.Mark;
            if (parityType == "Space")
                return Parity.Space;
            else
                return Parity.None;
        }
    }
}