using Windows.UI;
using Color = Windows.UI.Color;
using ColorHelper = CommunityToolkit.WinUI.Helpers.ColorHelper;

namespace ClassevivaPCTO.Helpers.Palettes
{
    internal abstract class Palettes
    {
        public class PaletteCvv : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#CF5955");
            public Color ColorOrange => ColorHelper.ToColor("#EE9B4C");
            public Color ColorGreen => ColorHelper.ToColor("#7EAE83");
            public Color ColorBlue => ColorHelper.ToColor("#5E97B2");
            public Color ColorYellow => ColorHelper.ToColor("#D8B83E");
        }

        public class PaletteCon : IPalette
        {
            public Color ColorRed => Colors.Crimson;
            public Color ColorOrange => Colors.DarkOrange;
            public Color ColorGreen => Colors.ForestGreen;
            public Color ColorBlue => Colors.Teal;
            public Color ColorYellow => Colors.Goldenrod;
        }

        public class PaletteNat : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#A25E50");
            public Color ColorOrange => ColorHelper.ToColor("#FC9553");
            public Color ColorGreen => ColorHelper.ToColor("#78C141");
            public Color ColorBlue => ColorHelper.ToColor("#42AACD");
            public Color ColorYellow => ColorHelper.ToColor("#D8B83E");
        }

        public class PaletteFlo : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#D3015A");
            public Color ColorOrange => ColorHelper.ToColor("#FE7E00");
            public Color ColorGreen => ColorHelper.ToColor("#09A854");
            public Color ColorBlue => ColorHelper.ToColor("#316595");
            public Color ColorYellow => ColorHelper.ToColor("#D8B83E");
        }

        public class PaletteRet : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#AA1841");
            public Color ColorOrange => ColorHelper.ToColor("#EE9B4C");
            public Color ColorGreen => ColorHelper.ToColor("#689F38");
            public Color ColorBlue => ColorHelper.ToColor("#1491D3");
            public Color ColorYellow => ColorHelper.ToColor("#FFB208");
        }

        public class Palette6 : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#D3212C");
            public Color ColorOrange => ColorHelper.ToColor("#FF681E");
            public Color ColorGreen => ColorHelper.ToColor("#069C56");
            public Color ColorBlue => ColorHelper.ToColor("#1491D3");
            public Color ColorYellow => ColorHelper.ToColor("#FF980E");
        }

        public class Palette7 : IPalette
        {
            public Color ColorRed => ColorHelper.ToColor("#8A2B13");
            public Color ColorOrange => ColorHelper.ToColor("#C4501B");
            public Color ColorGreen => ColorHelper.ToColor("#4E7145");
            public Color ColorBlue => ColorHelper.ToColor("#48877F");
            public Color ColorYellow => ColorHelper.ToColor("#CB8325");
        }
    }
}