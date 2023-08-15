using ClassevivaPCTO.Utils;
using Windows.UI;
using Windows.UI.Xaml.Media;
using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;

namespace ClassevivaPCTO.Adapters
{
    public class ScrutinioAdapter
    {
        public readonly ScrutiniDocument CurrentObject;

        private IPalette _currentPalette = PaletteSelectorService.PaletteClass;

        public SolidColorBrush StatusTextColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();

                if (CurrentObject.checkStatus.available)
                {
                    brush.Color = _currentPalette.ColorGreen;
                }
                else
                {
                    brush.Color = _currentPalette.ColorRed;
                }

                return brush;
            }
        }

        public string StatusText
        {
            get
            {
                if (CurrentObject.checkStatus.available)
                {
                    return " ";
                }

                return "Non disponibile";
            }
        }


        public ScrutinioAdapter(ScrutiniDocument ev)
        {
            CurrentObject = ev;
        }
    }
}
