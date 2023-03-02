using ClassevivaPCTO.Utils;
using Refit;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Security.Credentials;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace ClassevivaPCTO
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();

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


            var loginCredential = new CredUtils().GetCredentialFromLocker();

            if (loginCredential != null)
            {
                // There is a credential stored in the locker.
                // Populate the Password property of the credential
                // for automatic login.
                loginCredential.RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota

                edittext_username.Text = loginCredential.UserName.ToString();
                edittext_password.Password = loginCredential.Password.ToString();

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

            try
            {
                buttonLogin.Visibility = Visibility.Collapsed;
                progresslogin.Visibility = Visibility.Visible;

                var api = RestService.For<IClassevivaAPI>("https://web.spaggiari.eu/rest/v1");

                var measurement = new LoginData
                {
                    Uid = edittext_username.Text,
                    Pass = edittext_password.Password,
                };


                LoginResult user = await api.LoginAsync(measurement);

                /*
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Login completato";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = "Benvenuto " + user.FirstName + " " + user.LastName;

                //var result = await dialog.ShowAsync();*/

                if ((bool)checkboxCredenziali.IsChecked)
                {
                    var vault = new PasswordVault();
                    vault.Add(new PasswordCredential("classevivapcto", edittext_username.Text, edittext_password.Password));
                }


                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(DashboardPage), user, new DrillInNavigationTransitionInfo());

            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Errore";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = "Errore durante il login. Controlla il nome utente e la password.";

                try
                {
                    var result = await dialog.ShowAsync();
                } catch(Exception e) {
                    System.Console.WriteLine(e.ToString());
                }


                buttonLogin.Visibility = Visibility.Visible;
                progresslogin.Visibility = Visibility.Collapsed;

            }
        }



       
    }
}
