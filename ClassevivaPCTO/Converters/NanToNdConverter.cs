using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ClassevivaPCTO.Converters
{
    public class NanToNdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Single.NaN)
                return "N/D";
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}