using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Helpers;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Security.Credentials;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace ClassevivaPCTO.Views
{
    public sealed partial class LoginPage : Page
    {
        private IClassevivaAPI apiWrapper;

        public LoginPage()
        {
            this.InitializeComponent();

            //var mediaPlayer = new MediaPlayer();
            //mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Audio/sweep.mp3"));
            //mediaPlayer.Play();


            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            //remove the solid-colored backgrounds behind the caption controls and system back button
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            //titolo title bar
            AppTitleTextBlock.Text = "Login - " + AppInfo.Current.DisplayInfo.DisplayName;

            Window.Current.SetTitleBar(AppTitleBar);

            //display app version
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            VersionTextBlock.Text =
                $"{appName} - {version.Major}.{version.Minor}.{version.Build}"; //.{version.Revision}

            var loginCredentials = new CredUtils().GetCredentialFromLocker();

            if (loginCredentials != null)
            {
                // There is a credential stored in the locker.
                // Populate the Password property of the credential
                // for automatic login.
                loginCredentials
                    .RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota

                edittext_username.Text = loginCredentials.UserName;
                edittext_password.Password = loginCredentials.Password;

                Task.Run(async () => { await DoLoginAsync(); });
            }

            loginGrid.KeyDown += Grid_KeyDown;
        }

        private async void Grid_KeyDown(object sender, KeyRoutedEventArgs args)
        {
            switch (args.Key)
            {
                case VirtualKey.Enter:

                    await Task.Run(async () => { await DoLoginAsync(); });

                    break;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () => { await DoLoginAsync(); });
        }

        private async Task DoLoginAsync()
        {
            string edituid = null;
            string editpass = null;
            bool checkboxCredenzialiChecked = false;

            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    buttonLogin.Visibility = Visibility.Collapsed;
                    progresslogin.Visibility = Visibility.Visible;

                    edituid = edittext_username.Text;
                    editpass = edittext_password.Password;
                    checkboxCredenzialiChecked = (bool)checkboxCredenziali.IsChecked;
                }
            );

            if (edituid.Equals("test") && editpass.Equals("test"))
            {
                Endpoint.CurrentEndpoint = Endpoint.Test;

                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Used mock test account");
            }
            else
            {
                Endpoint.CurrentEndpoint = Endpoint.Official;

                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Used real account");
            }

            //get the refit api client from the service provider only after having set the endpoint
            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);

            try
            {
                var measurement = new LoginData
                {
                    Uid = edituid,
                    Pass = editpass,
                    Ident = null
                };

                var resLogin = await CvUtils.GetApiLoginData(apiWrapper, measurement);

                if (resLogin is LoginResultComplete loginResult)
                {
                    //check if the first letter of loginResult.ident is not letters S, X and G
                    if (loginResult.ident[0] != 'S' && loginResult.ident[0] != 'X' && loginResult.ident[0] != 'G')
                    {
                        Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Used TEACHER account");

                        await CoreApplication.MainView.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            async () =>
                            {
                                ContentDialog dialog = new ContentDialog();
                                dialog.Title = "Account insegnante non supportato";
                                dialog.PrimaryButtonText = "OK";
                                dialog.DefaultButton = ContentDialogButton.Primary;
                                dialog.Content =
                                    "In questa app sono solo supportati gli account studente. \n\n" +
                                    "Gli account studente hanno l'username che inizia con le lettere S, G o X.";

                                try
                                {
                                    var result = await dialog.ShowAsync();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            });


                        return;
                    }


                    GetUserDataAndGoAhead(loginResult, measurement, checkboxCredenzialiChecked);
                }
                else if (resLogin is LoginResultChoices loginResultChoices)
                {
                    LoginChoice resLoginChoice = null;

                    if (ChoiceSaverService.LoadChoiceIdentAsync().Result != null)
                    {
                        resLoginChoice = loginResultChoices.choices.Find(
                            x => x.ident == ChoiceSaverService.LoadChoiceIdentAsync().Result
                        );
                    }
                    else
                    {
                        var resultDialog = await ShowChoicesDialog(loginResultChoices);

                        if (resultDialog.Item1 == ContentDialogResult.Primary)
                        {
                            //get the chosen index from the dialog combobox
                            resLoginChoice = loginResultChoices.choices[
                                resultDialog.Item2.chosenIndex
                            ];
                        }
                        else if (resultDialog.Item1 == ContentDialogResult.None)
                        {
                            return;
                        }
                    }

                    var loginData = new LoginData
                    {
                        Uid = edituid,
                        Pass = editpass,
                        Ident = resLoginChoice.ident
                    };

                    var resLoginFinal = await CvUtils.GetApiLoginData(apiWrapper, loginData);

                    if (resLoginFinal is LoginResultComplete loginResultChoice)
                    {
                        GetUserDataAndGoAhead(
                            loginResultChoice,
                            loginData,
                            checkboxCredenzialiChecked,
                            resLoginChoice
                        );
                    }
                }
            }
            catch (ApiException ex)
            {
                //var message = ex.GetContentAsAsync<CvError>();

                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        ContentDialog dialog = new ContentDialog();
                        dialog.Title = "Errore";
                        dialog.PrimaryButtonText = "OK";
                        dialog.DefaultButton = ContentDialogButton.Primary;
                        dialog.Content =
                            "Errore durante il login. Controlla il nome utente e la password. \n Errore: "
                            + ex.Content;

                        try
                        {
                            var result = await dialog.ShowAsync();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        buttonLogin.Visibility = Visibility.Visible;
                        progresslogin.Visibility = Visibility.Collapsed;
                    }
                );
            }
        }

        private async void GetUserDataAndGoAhead(
            LoginResultComplete loginResultComplete,
            LoginData loginData,
            bool saveCredentials,
            LoginChoice loginChoice = null
        )
        {
            ViewModelHolder.GetViewModel().LoginResult = loginResultComplete;

            string fixedId = new CvUtils().GetCode(loginResultComplete.ident);

            CardsResult cardsResult = await apiWrapper.GetCards(
                fixedId,
                loginResultComplete.token
            );

            SingleCardResult singleCardResult = await apiWrapper.GetCardSingle(fixedId,
                loginResultComplete.token);


            ViewModelHolder.GetViewModel().SingleCardResult = singleCardResult.Card;
            ViewModelHolder.GetViewModel().CardsResult = cardsResult;

            if (saveCredentials)
            {
                var vault = new PasswordVault();
                vault.Add(new PasswordCredential("classevivapcto", loginData.Uid, loginData.Pass));

                if (loginChoice != null)
                {
                    await ChoiceSaverService.SaveChoiceIdentAsync(loginData.Ident);
                }
            }

            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    Frame rootFrame = (Frame)Window.Current.Content;
                    rootFrame.Navigate(
                        typeof(MainPage),
                        null,
                        new DrillInNavigationTransitionInfo()
                    );
                }
            );
        }


        private async Task<(ContentDialogResult, ChoiceDialogContent)> ShowChoicesDialog(
            LoginResultChoices loginResultChoices
        )
        {
            ContentDialogResult? result = null;
            ChoiceDialogContent contentDialogContent = null;

            TaskCompletionSource<bool> isSomethingLoading = new TaskCompletionSource<bool>();

            //make sure we are executing it on the main thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    contentDialogContent = new ChoiceDialogContent(loginResultChoices.choices);

                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "Scegli un profilo";
                    dialog.PrimaryButtonText = "Accedi";
                    dialog.CloseButtonText = "Annulla";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    dialog.RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
                    dialog.Content = contentDialogContent;

                    result = await dialog.ShowAsync();

                    isSomethingLoading.SetResult(true);
                }
            );

            await isSomethingLoading.Task;

            return ((ContentDialogResult)result, contentDialogContent);
        }
    }
}