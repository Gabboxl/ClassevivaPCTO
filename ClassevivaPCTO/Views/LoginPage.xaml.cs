using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

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
            //var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;

            //remove the solid-colored backgrounds behind the caption controls and system back button
            //ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            //titleBar.ButtonBackgroundColor = Colors.Transparent;
            //titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            //titolo title bar
            AppTitleTextBlock.Text = "Login - " + AppInfo.Current.DisplayInfo.DisplayName;

            //MainWindow.Current.SetTitleBar(AppTitleBar);


            this.Loaded += LoginPage_Loaded; //we execute some actions when the page is loaded
            //dialogs can only be shown after the page is loaded


            /*

            //the dispatcher can be useful to execute code as soon as the thread is ready and so the page is loaded
            Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(
    Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
    new Microsoft.UI.Dispatching.DispatcherQueueHandler(() =>
    {
        //roba();
    }));

            */




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

        private async void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            roba();
        }

        private async void roba()
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "test";
            dialog.PrimaryButtonText = "OK";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "Firstrun: " + SystemInformation.Instance.IsFirstRun;

            if (SystemInformation.Instance.IsFirstRun)
            {

                var result = await dialog.ShowAsync();
            }

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



                var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);

                var measurement = new LoginData
                {
                    Uid = edituid,
                    Pass = editpass,
                };


                LoginResult loginResult = await api.LoginAsync(measurement);


                ViewModelHolder.getViewModel().LoginResult = loginResult;


                string fixedId = new CvUtils().GetCode(loginResult.Ident);
                CardsResult cardsResult = await api.GetCards(fixedId, loginResult.Token.ToString());

                ViewModelHolder.getViewModel().CardsResult = cardsResult;


                if ((bool)checkboxCredenziali.IsChecked)
                {
                    var vault = new PasswordVault();
                    vault.Add(new PasswordCredential("classevivapcto", edittext_username.Text, edittext_password.Password));
                }


               

                Frame rootFrame = App.Window.Content as Frame;
                rootFrame.Navigate(typeof(MainPage), null, new DrillInNavigationTransitionInfo());

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
                    var result = await /* TODO You should replace 'this' with the instance of UserControl that is ContentDialog is meant to be a part of. */SetContentDialogRoot(dialog, this).ShowAsync();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }


                buttonLogin.Visibility = Visibility.Visible;
                progresslogin.Visibility = Visibility.Collapsed;

            }

        }
                    private static ContentDialog SetContentDialogRoot(ContentDialog contentDialog, UserControl control)
                    {
                        if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
                        {
                            contentDialog.XamlRoot = control.Content.XamlRoot;
                        }
                        return contentDialog;
                    }




    }
}
