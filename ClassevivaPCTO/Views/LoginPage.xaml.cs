using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Helpers;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.Media.Playback;
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

            var mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Audio/sweep.mp3"));
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

            VersionTextBlock.Text = $"{appName} - {version.Major}.{version.Minor}.{version.Build}"; //.{version.Revision}

            var loginCredentials = new CredUtils().GetCredentialFromLocker();

            if (loginCredentials != null)
            {
                // There is a credential stored in the locker.
                // Populate the Password property of the credential
                // for automatic login.
                loginCredentials.RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota

                edittext_username.Text = loginCredentials.UserName.ToString();
                edittext_password.Password = loginCredentials.Password.ToString();

                Task.Run(async () =>
                {
                    await doLoginAsync();
                });
                //doLoginAsync();
            }

            loginGrid.KeyDown += grid_KeyDown;
        }

        private async void grid_KeyDown(object sender, KeyRoutedEventArgs args)
        {
            switch (args.Key)
            {
                case VirtualKey.Enter:

                    await Task.Run(async () =>
                    {
                        await doLoginAsync();
                    });

                    break;
                default:
                    break;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                await doLoginAsync();
            });
        }

        public async Task doLoginAsync()
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

                var resLogin = await GetLoginData(measurement);

                if (resLogin is LoginResultComplete loginResult)
                {
                    DoFinalLogin(loginResult, measurement, checkboxCredenzialiChecked);
                }
                else if (resLogin is LoginResultChoices loginResultChoices)
                {
                    LoginChoice resloginChoice = null;

                    if (ChoiceSaverService.LoadChoiceIdentAsync().Result != null)
                    {
                        resloginChoice = loginResultChoices.choices.Find(
                            x => x.ident == ChoiceSaverService.LoadChoiceIdentAsync().Result
                        );
                    }
                    else
                    {
                        var resultDialog = await ShowChoicesDialog(loginResultChoices);

                        if (resultDialog.Item1 == ContentDialogResult.Primary)
                        {
                            //get the chosen index from the dialog combobox
                            resloginChoice = loginResultChoices.choices[
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
                        Ident = resloginChoice.ident
                    };

                    var resLoginFinal = await GetLoginData(loginData);

                    if (resLoginFinal is LoginResultComplete loginResultChoice)
                    {
                        DoFinalLogin(
                            loginResultChoice,
                            loginData,
                            checkboxCredenzialiChecked,
                            resloginChoice
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
                            System.Console.WriteLine(e.ToString());
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

        public async void DoFinalLogin(
            LoginResultComplete loginResultComplete,
            LoginData loginData,
            bool saveCredentials,
            LoginChoice loginChoice = null
        )
        {
            ViewModelHolder.getViewModel().LoginResult = loginResultComplete;

            string fixedId = new CvUtils().GetCode(loginResultComplete.ident);

            CardsResult cardsResult = await apiWrapper.GetCards(
                fixedId,
                loginResultComplete.token.ToString()
            );

            SingleCardResult singleCardResult = await apiWrapper.GetCardSingle(fixedId,
                loginResultComplete.token.ToString());


            ViewModelHolder.getViewModel().SingleCardResult = singleCardResult.Card;
            ViewModelHolder.getViewModel().CardsResult = cardsResult;

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
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(
                        typeof(MainPage),
                        null,
                        new DrillInNavigationTransitionInfo()
                    );
                }
            );
        }

        public async Task<object> GetLoginData(LoginData loginData)
        {
            var res = await apiWrapper.LoginAsync(loginData);

            // Code to execute after the API call
            Console.WriteLine("Executing some code after API call");

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = await res.Content.ReadAsStringAsync();

                if (responseContent.Contains("choice"))
                {
                    LoginResultChoices loginResultchoices =
                        JsonConvert.DeserializeObject<LoginResultChoices>(responseContent);
                    return loginResultchoices;
                }
                else
                {
                    LoginResultComplete loginResult =
                        JsonConvert.DeserializeObject<LoginResultComplete>(responseContent);
                    return loginResult;
                }
            }
            else
            {
                // Create RefitSettings object
                RefitSettings refitSettings = new RefitSettings();

                // Create ApiException with request and response details
                throw await ApiException.Create(
                    res.RequestMessage,
                    HttpMethod.Get,
                    res,
                    refitSettings
                );
            }
        }

        private async Task<(ContentDialogResult, ChoiceDialogContent)> ShowChoicesDialog(
            LoginResultChoices loginResultChoices
        )
        {
            ContentDialogResult? result = null;
            ChoiceDialogContent contentDialogContent = null;

            TaskCompletionSource<bool> IsSomethingLoading = new TaskCompletionSource<bool>();

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
                    dialog.RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
                    dialog.Content = contentDialogContent;

                    result = await dialog.ShowAsync();

                    IsSomethingLoading.SetResult(true);
                }
            );

            await IsSomethingLoading.Task;

            return ((ContentDialogResult)result, contentDialogContent);
        }
    }
}
