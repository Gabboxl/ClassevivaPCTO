using ClassevivaPCTO.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace ClassevivaPCTO.Services
{
    public static class ChoiceSaverService
    {
        private const string SettingsKey = "AppLoginAccountChoice";

        private static async Task<int?> LoadThemeFromSettingsAsync()
        {
            int? choice = await ApplicationData.Current.LocalSettings.ReadAsync<int>(SettingsKey);

            return choice;
        }

        private static async Task SaveThemeInSettingsAsync(int choice)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, choice.ToString());
        }
    }
}
