﻿using ClassevivaPCTO.Helpers;
using ClassevivaPCTO.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Helpers.Palettes;
using Microsoft.Extensions.DependencyInjection;

namespace ClassevivaPCTO.Views
{
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }
            set { Set(ref _elementTheme, value); }
        }

        

        private PaletteType _paletteType = PaletteSelectorService.PaletteEnum;

        public PaletteType PaletteType
        {
            get { return _paletteType; }
            set { Set(ref _paletteType, value); }
        }

        public string AppName
        {
            get { return "AppDisplayName".GetLocalized(); }
        }

        private string _version;

        public string Version
        {
            get { return _version; }
            set { Set(ref _version, value); }
        }

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            Version = GetVersionDescription();

            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void HyperlinkVote_Click(object sender, RoutedEventArgs e)
        {
            //https://learn.microsoft.com/en-us/windows/uwp/monetize/request-ratings-and-reviews

            StoreContext storeContext = StoreContext.GetDefault();
            StoreRateAndReviewResult result = await storeContext.RequestRateAndReviewAppAsync();


            switch (result.Status)
            {
                case StoreRateAndReviewStatus.Succeeded:
                    // Was this an updated review or a new review, if Updated is false it means it was a users first time reviewing
                    if (result.WasUpdated)
                    {
                        // This was an updated review thank user
                        //ThankUserForReview(); // pseudo-code
                    }
                    else
                    {
                        // This was a new review, thank user for reviewing and give some free in app tokens
                        //ThankUserForReviewAndGrantTokens(); // pseudo-code
                    }

                    // Keep track that we prompted user and don’t do it again for a while
                    //SetUserHasBeenPrompted(); // pseudo-code
                    break;

                case StoreRateAndReviewStatus.CanceledByUser:
                    // Keep track that we prompted user and don’t prompt again for a while
                    //SetUserHasBeenPrompted(); // pseudo-code

                    break;

                case StoreRateAndReviewStatus.NetworkError:
                    // User is probably not connected, so we’ll try again, but keep track so we don’t try too often
                    //SetUserHasBeenPromptedButHadNetworkError(); // pseudo-code

                    break;

                // Something else went wrong
                case StoreRateAndReviewStatus.Error:
                default:
                    // Log error, passing in ExtendedJsonData however it will be empty for now
                    Debug.WriteLine(result.ExtendedError, result.ExtendedJsonData);
                    break;
            }
        }

        private async void ButtonChangelog_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new WhatsNewDialog();
            await dialog.ShowAsync();
        }

        private async void ThemeSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox themeSelector = (ComboBox) sender;

            //change theme based on selected index of the combobox sender
            await ThemeSelectorService.SetThemeAsync((ElementTheme)themeSelector.SelectedIndex);

        }

        private async void PaletteChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await PaletteSelectorService.SetCurrentPalette((PaletteType)param);
            }

        }
    }
}