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
        public object Convert(object value, Type targetType, object? parameter, string language)
        {
            SolidColorBrush brush = new();

            IPalette currentPalette = PaletteSelectorService.PaletteClass;

            float? valore = VariousUtils.GradeToFloat(value);

            switch (valore)
            {
                case null:
                    brush.Color = currentPalette.ColorBlue;
                    break;
                case >= 6:
                    brush.Color = currentPalette.ColorGreen;
                    break;
                case >= 5:
                    brush.Color = currentPalette.ColorOrange;
                    break;
                default:
                {
                    if (float.IsNaN((float) valore))
                    {
                        brush = (SolidColorBrush) Application.Current.Resources["TextFillColorTertiaryBrush"];
                    }
                    else
                    {
                        brush.Color = currentPalette.ColorRed;
                    }

                    break;
                }
            }

            //if parameter equals 1 int
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