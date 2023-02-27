using ClassevivaPCTO.Utils;
using Refit;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace ClassevivaPCTO
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();


        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            IClassevivaAPI api = RestService.For<IClassevivaAPI>("https://web.spaggiari.eu/rest/v1");

            var measurement = new LoginData
            {
                Uid = "",
                Pass = "",
            };

            LoginResult user = api.LoginAsync(measurement).Result;




            MessageDialog dialog = new MessageDialog("a " + user.FirstName + user.LastName);
            dialog.Commands.Add(new UICommand("Yes", null));
            dialog.Commands.Add(new UICommand("No", null));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var cmd = await dialog.ShowAsync();

        }
    }
}
