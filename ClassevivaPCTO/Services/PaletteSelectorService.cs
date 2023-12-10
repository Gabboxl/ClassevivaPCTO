using ClassevivaPCTO.Helpers;
using ClassevivaPCTO.Helpers.Palettes;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace ClassevivaPCTO.Services
{
    public static class PaletteSelectorService
    {
        private const string SettingsKey = "CurrentPalette";

        //private IPalette _currentPalette;

        public static IPalette PaletteClass { get; set; } = new Palettes.PaletteCvv();
        public static PaletteType PaletteEnum { get; set; } = PaletteType.PALETTE_CVV;

        public static async Task InitializeAsync()
        {
            PaletteEnum = await GetCurrentPaletteEnum();
            PaletteClass = GetPaletteClass(PaletteEnum);
        }

        public static async Task SetCurrentPalette(PaletteType palette)
        {
            PaletteEnum = palette;
            PaletteClass = GetPaletteClass(palette);

            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, palette.ToString());
        }

        public static IPalette GetPaletteClass(PaletteType classType)
        {
            var fieldInfo = typeof(PaletteType).GetField(classType.ToString());
            var classMappingAttribute = fieldInfo.GetCustomAttribute<ClassMappingAttribute>();

            if (classMappingAttribute != null)
            {
                Type classTypeToInstantiate = classMappingAttribute.ClassType;
                return (IPalette) Activator.CreateInstance(classTypeToInstantiate);
            }

            throw new ArgumentException("Invalid ClassType value.");
        }

        private static async Task<PaletteType> GetCurrentPaletteEnum()
        {
            PaletteType cacheTheme = PaletteType.PALETTE_CVV; //default palette
            string themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        /*
        public static async Task<IPalette> GetCurrentPaletteClass()
        {
            return GetPaletteClass(await GetCurrentPaletteEnum());
        }
        */
    }
}