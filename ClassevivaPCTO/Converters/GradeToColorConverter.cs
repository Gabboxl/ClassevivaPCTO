using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ClassevivaPCTO.Converters
{
    public class GradeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object? parameter, string language)
        {
            SolidColorBrush brush = new();

            IPalette currentPalette = PaletteSelectorService.PaletteClass;

            float? valore = VariousUtils.GradeToFloat(value);

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
            else if (float.IsNaN((float) valore) || valore == 0)
            {
                // set brush to staticresource TextFillColorTertiaryBrush's Color (not the brush per se, otherwise it would be a reference and if we modify its RGB data it applies to all the usages in the app!)
                var TextFillColorTertiaryBrush = (SolidColorBrush) Application.Current.Resources["TextFillColorTertiaryBrush"];
                brush.Color = TextFillColorTertiaryBrush.Color;
            }
            else
            {
                brush.Color = currentPalette.ColorRed;
            }

            //if parameter equals 1 int value, darken the color (for background use)
            if (parameter != null && int.TryParse((string) parameter, out int param))
            {
                if (param == 1)
                {
                    brush.Color = VariousUtils.DarkenColor(brush.Color, 0.5);
                }
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}