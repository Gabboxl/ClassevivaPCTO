using ClassevivaPCTO.Utils;
using Refit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace ClassevivaPCTO
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();


        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            buttonLogin.Visibility = Visibility.Collapsed;
            progresslogin.Visibility = Visibility.Visible;


            IClassevivaAPI api = RestService.For<IClassevivaAPI>("https://web.spaggiari.eu/rest/v1");

            var measurement = new LoginData
            {
                Uid = edittext_username.Text,
                Pass = edittext_password.Password,
            };

            LoginResult user = api.LoginAsync(measurement).Result;

            

            Thread.Sleep(2000);


            
            MessageDialog dialog = new MessageDialog("a " + user.FirstName + user.LastName);
            dialog.Commands.Add(new UICommand("Yes", null));
            dialog.Commands.Add(new UICommand("No", null));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var cmd = await dialog.ShowAsync();
            


            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(DashboardPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
