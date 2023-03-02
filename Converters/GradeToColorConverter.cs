using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Converters
{
    public class GradeToColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                SolidColorBrush brush = new SolidColorBrush();
                float grade = (float)value;

                if (grade >= 6)
                {
                    brush.Color = Colors.Teal;
                }
                else if(grade >= 5)
            {
                brush.Color = Colors.Orange;
            } else
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
