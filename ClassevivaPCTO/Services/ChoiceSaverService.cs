using ClassevivaPCTO.Helpers;
using System.Threading.Tasks;
using Windows.Storage;

namespace ClassevivaPCTO.Services
{
    public static class ChoiceSaverService
    {
        private const string SettingsKey = "AppLoginChoiceIdent";

        public static async Task<string?> LoadChoiceIdentAsync()
        {
            string? ident = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            return ident;
        }

        public static async Task SaveChoiceIdentAsync(string? ident)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, ident.ToString());
        }

        public static void RemoveSavedChoiceIdent()
        {
            ApplicationData.Current.LocalSettings.RemoveKey(SettingsKey);
        }
    }
}