using ClassevivaPCTO.Utils;
using Windows.UI.Xaml.Media;
using ClassevivaPCTO.Helpers;
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
                SolidColorBrush brush = new()
                {
                    Color = CurrentObject.checkStatus.available ? _currentPalette.ColorGreen : _currentPalette.ColorRed
                };

                return brush;
            }
        }

        public string StatusText
        {
            get
            {
                return CurrentObject.checkStatus.available ? "" : "ScrutiniNotAvailableStatusText".GetLocalizedStr();
            }
        }

        public ScrutinioAdapter(ScrutiniDocument ev)
        {
            CurrentObject = ev;
        }
    }
}