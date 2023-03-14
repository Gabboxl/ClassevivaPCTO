using ClassevivaPCTO.Utils;
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

            float? valore = null;

            if (value is Grade)
            {

                Grade grade = (Grade)value;


                if (grade.decimalValue != null)
                {
                    valore = grade.decimalValue;
                }
                else if (!grade.displayValue.ToLower().Equals("nv"))
                {
                    switch (grade.displayValue)
                    {
                        case "o":
                            valore = 10;
                            break;

                        case "ds":
                            valore = 9;
                            break;

                        case "b":
                            valore = 8;
                            break;

                        case "dc":
                            valore = 7;
                            break;

                        case "s":
                            valore = 6;
                            break;

                        case "i":
                            valore = 5;
                            break;
                    }
                }
            } else
            {
                valore = (float?)value;
            }

            if (valore == null) {
                brush.Color = Colors.DarkSlateBlue;

                 } else if (valore >= 6)
                {
                    brush.Color = Colors.Teal;
                }
                else if(valore >= 5)
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
