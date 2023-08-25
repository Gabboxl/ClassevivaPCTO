using System;
using Windows.UI.Xaml.Data;

namespace ClassevivaPCTO.Converters
{
    public class DateTimeToHourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime date = (DateTime) value;

            return date.ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}