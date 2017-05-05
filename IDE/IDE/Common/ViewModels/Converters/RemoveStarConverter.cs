using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class RemoveStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string original = (string)value;
            return original.Replace("*", string.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //pointless
            string original = (string)value;
            return original.Replace("*", string.Empty);
        }
    }
}
