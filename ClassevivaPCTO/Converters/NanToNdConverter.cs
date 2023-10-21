using ClassevivaPCTO.Helpers;
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
            if (value is float.NaN)
                return "GradesNotAvailable".GetLocalized();
            else if (value is float numberValue)
                return numberValue.ToString("0.0");

            return new InvalidOperationException("Value must be a float or NaN");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}