using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    [ValueConversion(typeof(ComboBoxItem), typeof(int))]
    public class ComboBoxItemToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ComboBoxItem() { Content = int.Parse(value?.ToString()) };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as ComboBoxItem;
            var content = item.Content.ToString();
            return int.Parse(Regex.Replace(content, @"[^\d]", ""));
        }
    }
}
