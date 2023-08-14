using Windows.UI;
using Color = Windows.UI.Color;
using ColorHelper = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;

namespace ClassevivaPCTO.Helpers.Palettes
{
    internal abstract class Palettes
    {
        public class PaletteCv : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#CF5955");
            public Color ColorOrange => ColorHelper.ToColor("#EE9B4C");
            public Color ColorYellow => ColorHelper.ToColor("#EE9B4C");
            public Color ColorGreen => ColorHelper.ToColor("#7EAE83");
            public Color ColorBlue => ColorHelper.ToColor("#5E97B2");

        }

        public class PaletteJap : IPalette
        {
            public Color ColorRed => Colors.Crimson;
            public Color ColorOrange => Colors.DarkOrange;
            public Color ColorYellow => Colors.Goldenrod;
            public Color ColorGreen => Colors.Teal;
            public Color ColorBlue => ColorHelper.ToColor("#483D8B");

        }

        public class PaletteNat : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#9A4D31");
            public Color ColorOrange => ColorHelper.ToColor("#FC9553");
            public Color ColorYellow => ColorHelper.ToColor("#FC9553");
            public Color ColorGreen => ColorHelper.ToColor("#78C141");
            public Color ColorBlue => ColorHelper.ToColor("#42AACD");
        }
    }
}