using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Converters
{
    public class GradeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SolidColorBrush brush = new();

            float? valore = null;

            IPalette currentPalette = PaletteSelectorService.PaletteClass;

            valore = VariousUtils.GradeToFloat(value);

            if (valore == null)
            {
                brush.Color = currentPalette.ColorBlue;
            }
            else if (valore >= 6)
            {
                brush.Color = currentPalette.ColorGreen;
            }
            else if (valore >= 5)
            {
                brush.Color = currentPalette.ColorOrange;
            }
            else if (float.IsNaN((float) valore))
            {
                // set brush to staticresource TextFillColorTertiaryBrush
                brush = (SolidColorBrush)Application.Current.Resources["TextFillColorTertiaryBrush"];

            }
            else
            {
                brush.Color = currentPalette.ColorRed;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}