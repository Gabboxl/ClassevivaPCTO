using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;
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

            IPalette CurrentPalette = PaletteSelectorService.PaletteClass;

            valore = VariousUtils.GradeToFloat(value);

            if (valore == null)
            {
                brush.Color = Colors.DarkSlateBlue;
            }
            else if (valore >= 6)
            {
                brush.Color = CurrentPalette.ColorGreen;
            }
            else if (valore >= 5)
            {
                brush.Color = CurrentPalette.ColorOrange;
            }
            else
            {
                brush.Color = CurrentPalette.ColorRed;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}