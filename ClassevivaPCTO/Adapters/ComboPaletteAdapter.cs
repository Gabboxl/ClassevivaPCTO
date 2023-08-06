using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Utils;

namespace ClassevivaPCTO.Adapters
{
    public class ComboPaletteAdapter
    {
        public readonly IPalette CurrentPalette;

        public readonly PaletteType CurrentPaletteType;

        public string PaletteNameTranslatedResource
        {
            get
            {
                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

                var paletteEnumName = CurrentPaletteType.ToString();

                //return the translated name of the palette with the name of the palette enum
                return resourceLoader.GetString(paletteEnumName);
            }
        }

        public ComboPaletteAdapter(IPalette palette, PaletteType paletteType)
        {
            CurrentPalette = palette;
            CurrentPaletteType = paletteType;
        }
    }
}
