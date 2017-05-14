using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    [ValueConversion(typeof(ComboBoxItem), typeof(string))]
    public class ComboBoxItemToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ComboBoxItem() { Content = value?.ToString() };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ComboBoxItem;
            return item.Content.ToString();
        }
    }
}
