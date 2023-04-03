using ClassevivaPCTO.Utils;
using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace ClassevivaPCTO.Converters
{
    public class GradeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush brush = new SolidColorBrush();

            float? valore = null;

            valore = VariousUtils.GradeToFloat(value);

            if (valore == null)
            {
                brush.Color = Colors.DarkSlateBlue;

            }
            else if (valore >= 6)
            {
                brush.Color = Colors.Teal;
            }
            else if (valore >= 5)
            {
                brush.Color = Colors.Orange;
            }
            else
            {
                brush.Color = Colors.Red;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }

}
