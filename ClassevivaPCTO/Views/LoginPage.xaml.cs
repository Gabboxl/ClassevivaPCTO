using ClassevivaPCTO.Controls.DataTemplates;
using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Refit;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Security.Credentials;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Reflection.Emit;
using Windows.Services.Maps;
using System.Diagnostics.Metrics;
using System.Net.Http;

namespace ClassevivaPCTO.Views
{
    public sealed partial class LoginPage : Page
    {
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


            var loginCredentials = new CredUtils().GetCredentialFromLocker();

            if (loginCredentials != null)
            {
                // There is a credential stored in the locker.
                // Populate the Password property of the credential
                // for automatic login.
                loginCredentials.RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota

                edittext_username.Text = loginCredentials.UserName.ToString();
                edittext_password.Password = loginCredentials.Password.ToString();

                doLoginAsync();
            }


            loginGrid.KeyDown += grid_KeyDown;
        }


        private async void grid_KeyDown(object sender, KeyRoutedEventArgs args)
        {
            switch (args.Key)
            {
                case VirtualKey.Enter:

                    await doLoginAsync();

                    break;
                default:
                    break;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            await doLoginAsync();

        }


        public async Task doLoginAsync()
        {
            string edituid = edittext_username.Text;
            string editpass = edittext_password.Password;

            if (edituid.Equals("test") && editpass.Equals("test"))
            {
                Endpoint.CurrentEndpoint = Endpoint.Test;
            }
            else
            {
                Endpoint.CurrentEndpoint = Endpoint.Official;
            }



            try
            {
                buttonLogin.Visibility = Visibility.Collapsed;
                progresslogin.Visibility = Visibility.Visible;



                var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint, new RefitSettings(
                    new NewtonsoftJsonContentSerializer(new JsonSerializerSettings { 
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Converters = { new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal } }


                    })));

                var measurement = new LoginData
                {
                    Uid = edituid,
                    Pass = editpass,
                };


                var resLogin = await LoginApi(api, measurement);


                if(resLogin is LoginResultComplete loginResult)
                {
                    ViewModelHolder.getViewModel().LoginResult = loginResult;


                    string fixedId = new CvUtils().GetCode(loginResult.ident);
                    CardsResult cardsResult = await api.GetCards(fixedId, loginResult.token.ToString());

                    ViewModelHolder.getViewModel().CardsResult = cardsResult;


                    if ((bool)checkboxCredenziali.IsChecked)
                    {
                        var vault = new PasswordVault();
                        vault.Add(new PasswordCredential("classevivapcto", edittext_username.Text, edittext_password.Password));
                    }


                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());

                } else if (resLogin is LoginResultChoices loginResultChoices) 
                {

                    ShowChoicesDialog(loginResultChoices);
                    buttonLogin.Visibility = Visibility.Visible;
                    progresslogin.Visibility = Visibility.Collapsed;
                }



            }
            catch (ApiException ex)
            {

                //var message = ex.GetContentAsAsync<CvError>();

                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Errore";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = "Errore durante il login. Controlla il nome utente e la password. \n Errore: " + ex.Content;

                try
                {
                    var result = await dialog.ShowAsync();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }


                buttonLogin.Visibility = Visibility.Visible;
                progresslogin.Visibility = Visibility.Collapsed;

            }

        }

        public async Task<object> LoginApi(IClassevivaAPI classevivaAPI, LoginData loginData)
        {

            var res = await classevivaAPI.LoginAsync(loginData);

            // Code to execute after the API call
            Console.WriteLine("Executing some code after API call");

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = await res.Content.ReadAsStringAsync();

                if (responseContent.Contains("choice"))
                {
                    LoginResultChoices loginResultchoices = JsonConvert.DeserializeObject<LoginResultChoices>(responseContent);
                    return loginResultchoices;
                }
                else
                {
                    LoginResultComplete loginResult = JsonConvert.DeserializeObject<LoginResultComplete>(responseContent);
                    return loginResult;
                }

            } else
            {
                // Create RefitSettings object
                RefitSettings refitSettings = new RefitSettings();

                // Create ApiException with request and response details
                throw await ApiException.Create(res.RequestMessage, HttpMethod.Get, res, refitSettings);


            }
        }



        private async void ShowChoicesDialog(LoginResultChoices loginResultChoices)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Scegli un profilo";
            dialog.PrimaryButtonText = "Accedi";
            dialog.CloseButtonText = "Annulla";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new ContentDialogContent(loginResultChoices.choices);

            

            var result = await dialog.ShowAsync();
        }


    }
}
