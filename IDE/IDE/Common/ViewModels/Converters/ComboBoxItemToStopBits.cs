using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    [ValueConversion(typeof(ComboBoxItem), typeof(StopBits))]
    public class ComboBoxItemToStopBits : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ComboBoxItem() { Content = value?.ToString() };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ComboBoxItem;
            var itemContent = item.Content.ToString();

            if (itemContent == "1")
                return StopBits.One;
            if (itemContent == "1.5")
                return StopBits.OnePointFive;
            if (itemContent == "2")
                return StopBits.Two;
            else
                return StopBits.None;
        }
    }
}