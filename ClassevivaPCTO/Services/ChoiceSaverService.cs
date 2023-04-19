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

        public static async Task<int?> LoadChoiceAsync()
        {
            int? choice = await ApplicationData.Current.LocalSettings.ReadAsync<int>(SettingsKey);

            return choice;
        }

        public static async Task SaveChoiceAsync(int choice)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, choice.ToString());
        }
    }
}
