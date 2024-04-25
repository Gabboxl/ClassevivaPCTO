using ClassevivaPCTO.Utils;
using System;
using Windows.UI.Xaml.Data;

namespace ClassevivaPCTO.Converters
{
    public class AbsenceTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return CvUtils.GetColorFromAbsenceCode((AbsenceEventCode) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}