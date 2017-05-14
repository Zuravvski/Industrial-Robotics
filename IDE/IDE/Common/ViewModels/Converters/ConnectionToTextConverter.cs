using System;
using System.Globalization;
using System.Windows.Data;

namespace IDE.Common.ViewModels.Converters
{
    public class ConnectionToTextConverter : IValueConverter
    {
        private const string CONNECTED = "Connected";
        private const string DISCONNECTED = "Disconnected";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connection = DISCONNECTED;
            if (value is bool)
            {
                var val = (bool) value;
                connection = val ? CONNECTED : DISCONNECTED;
            }
            return connection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
