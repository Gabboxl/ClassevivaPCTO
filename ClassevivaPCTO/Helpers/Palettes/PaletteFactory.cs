using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassevivaPCTO.Helpers.Palettes
{
    internal class PaletteFactory
    {
        private const string SettingsKey = "CurrentPalette";

        //private IPalette _currentPalette;

        public async Task<PaletteType> GetCurrentPaletteEnum()
        {
            PaletteType cacheTheme = PaletteType.PALETTE_CV; //default palette
            string themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }


        public async Task SetCurrentPalette(PaletteType palette)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, palette.ToString());
        }


        private IPalette GetPaletteClass(PaletteType classType)
        {
            var fieldInfo = typeof(PaletteType).GetField(classType.ToString());
            var classMappingAttribute = fieldInfo.GetCustomAttribute<ClassMappingAttribute>();

            if (classMappingAttribute != null)
            {
                Type classTypeToInstantiate = classMappingAttribute.ClassType;
                return (IPalette)Activator.CreateInstance(classTypeToInstantiate);
            }

            throw new ArgumentException("Invalid ClassType value.");
        }


        public async Task<IPalette> GetCurrentPalette()
        {
            return GetPaletteClass(await GetCurrentPaletteEnum());
        }
    }
}
