﻿using ClassevivaPCTO.Helpers;
using ClassevivaPCTO.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Utils;
using Windows.Globalization;
using Crowdin.Api;
using Crowdin.Api.ProjectsGroups;
using Crowdin.Api.TranslationStatus;

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

        private List<ComboPaletteAdapter> _comboPalettes;

        public List<ComboPaletteAdapter> ComboPalettes
        {
            get { return _comboPalettes; }
            set { Set(ref _comboPalettes, value); }
        }

        private List<string> ComboLanguages
        {
            get
            {
                //for every language of the manifest create a new string list with full names of the languages
                //ApplicationLanguages.ManifestLanguages.ToList();

                List<string> languages = new List<string>();
                foreach (string language in ApplicationLanguages.ManifestLanguages)
                {
                    languages.Add(new Language(language).DisplayName);
                }

                return languages;
            }
        }

        private static int CurrentLanguage
        {
            get
            {
                return ApplicationLanguages.ManifestLanguages.ToList().IndexOf(ApplicationLanguages.Languages.First());
            }
            set { }
        }

        public static string AppName
        {
            get { return "AppDisplayName".GetLocalizedStr(); }
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

            LanguageComboBox.ItemsSource = ComboLanguages;
            LanguageComboBox.SelectedIndex = CurrentLanguage;
            LanguageComboBox.SelectionChanged += LanguageComboBox_OnSelectionChanged;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeAsync();
        }


        private async Task InitializeAsync()
        {
            Version = GetVersionDescription();

            //create combo palette adapters for each palette of the enum PaletteType
            ComboPalettes = new List<ComboPaletteAdapter>();
            foreach (PaletteType paletteType in Enum.GetValues(typeof(PaletteType)))
            {
                ComboPalettes.Add(new ComboPaletteAdapter(PaletteSelectorService.GetPaletteClass(paletteType),
                    paletteType));
            }


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

        private async void ButtonTutorial_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FirstRunDialog();
            await dialog.ShowAsync();
        }

        private async void ThemeSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox themeSelector = (ComboBox) sender;

            //change theme based on selected index of the combobox sender
            await ThemeSelectorService.SetThemeAsync((ElementTheme) themeSelector.SelectedIndex);
        }

        /*private async void PaletteChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await PaletteSelectorService.SetCurrentPalette((PaletteType)param);
            }

        }*/

        private async void PaletteComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox paletteSelector = (ComboBox) sender;

            //change theme based on selected index of the combobox sender
            PaletteType = (PaletteType) paletteSelector.SelectedIndex;
            await PaletteSelectorService.SetCurrentPalette(PaletteType);
        }

        private async void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "AreYouSure".GetLocalizedStr(),
                Content = "AreYouSureToExit".GetLocalizedStr(),
                PrimaryButtonText = "Exit".GetLocalizedStr(),
                CloseButtonText = "CancelDialogButton".GetLocalizedStr(),
                RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme,
                DefaultButton = ContentDialogButton.Primary
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                VariousUtils.DoLogout();
            }
        }

        private async void ChangeLanguage(int indexValue)
        {
            ApplicationLanguages.PrimaryLanguageOverride = ApplicationLanguages.ManifestLanguages[indexValue];

            //set selected index to the new value
            CurrentLanguage = indexValue;
        }

        private async void LanguageComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox languageSelector = (ComboBox) sender;
            var selectedIndex = languageSelector.SelectedIndex;


            if (selectedIndex == -1)
            {
                return;
            }

            if (selectedIndex != 0)
            {
                string langcode = ApplicationLanguages.ManifestLanguages[selectedIndex].ToLower();


                var credentials = new CrowdinCredentials
                {
                    AccessToken = "60bf870634938d9ef6f0dfb831748dfced1fb6000452405fc3df563f94d2942ec98454c90a524674"
                };
                var client = new CrowdinApiClient(credentials);

                var projectexecutor = new ProjectsGroupsApiExecutor(client);
                var projectBase = await projectexecutor.GetProject<ProjectBase>(605451);


                //from the TargetLanguages property of the projectBase object, create a list only of the Locale property of each object

                //var localelist = projectBase.TargetLanguages.Select(x => x.Locale).ToList();


                var selectedCrowdingLangId = projectBase.TargetLanguages.Where(x => x.Locale.ToLower() == langcode)
                    .Select(x => x.Id).FirstOrDefault();


                var languageProgressObj =
                    await new TranslationStatusApiExecutor(client).GetLanguageProgress(605451, selectedCrowdingLangId);

                var langProgressPerc = languageProgressObj.Data[0].TranslationProgress;


                if (langProgressPerc != 100)
                {
                    ContentDialog dialogtrans = new()
                    {
                        Title = "CautionDialogTitle".GetLocalizedStr(),
                        Content = "AppTranslationStatus1".GetLocalizedStr() + langProgressPerc + "% " +
                                  "AppTranslationStatus2".GetLocalizedStr() + "\n\n" +
                                  "AreYouSureLanguageDialogText".GetLocalizedStr(),
                        PrimaryButtonText = "ContinueDialogButton".GetLocalizedStr(),
                        CloseButtonText = "CancelDialogButton".GetLocalizedStr(),
                        RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme,
                        DefaultButton = ContentDialogButton.Primary
                    };

                    var resTransChoice = await dialogtrans.ShowAsync();

                    if (resTransChoice == ContentDialogResult.Primary)
                    {
                    }
                    else
                    {
                        //do not trigger this event again
                        LanguageComboBox.SelectionChanged -= LanguageComboBox_OnSelectionChanged;

                        //set previous selected value
                        LanguageComboBox.SelectedIndex = CurrentLanguage;

                        //re-add listener
                        LanguageComboBox.SelectionChanged += LanguageComboBox_OnSelectionChanged;

                        return;
                    }
                }
                else if (langProgressPerc == 0)
                {
                    ContentDialog dialogtrans = new()
                    {
                        Title = "Attenzione",
                        Content = "L'app non è stata ancora tradotta nella lingua che hai scelto.",
                        PrimaryButtonText = "Ok",
                        RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme,
                        DefaultButton = ContentDialogButton.Primary
                    };

                    await dialogtrans.ShowAsync();

                    return;
                }
            }

            //update values
            ChangeLanguage(selectedIndex);

            ContentDialog dialog = new()
            {
                Title = "RestartRequired".GetLocalizedStr(),
                Content = "RestartRequiredLanguageChange".GetLocalizedStr(),
                PrimaryButtonText = "Restart".GetLocalizedStr(),
                CloseButtonText = "CancelDialogButton".GetLocalizedStr(),
                RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme,
                DefaultButton = ContentDialogButton.Primary
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                CoreApplication.RequestRestartAsync("LanguageChangeRestart");
            }
        }
    }
}