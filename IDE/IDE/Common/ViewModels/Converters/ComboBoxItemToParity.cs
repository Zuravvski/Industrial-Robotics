using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    [ValueConversion(typeof(ComboBoxItem), typeof(Parity))]
    public class ComboBoxItemToParity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ComboBoxItem() { Content = value?.ToString() };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ComboBoxItem;
            var itemContent = item.Content.ToString();

            if (itemContent == "Odd")
                return Parity.Odd;
            if (itemContent == "Even")
                return Parity.Even;
            if (itemContent == "Mark")
                return Parity.Mark;
            if (itemContent == "Space")
                return Parity.Space;
            else
                return Parity.None;
        }
    }
}