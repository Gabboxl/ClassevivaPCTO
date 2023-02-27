using ClassevivaPCTO.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassevivaPCTO
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            this.InitializeComponent();


            Window.Current.SetTitleBar(AppTitleBar);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoginResult parameters = (LoginResult)e.Parameter;

            textBenvenuto.Text = "Benvenuto " + UppercaseFirst(parameters.FirstName) + " " + UppercaseFirst(parameters.LastName);
            

        }




        private async void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {

            var loginCredential = new CredUtils().GetCredentialFromLocker();

            if (loginCredential != null)
            {
                loginCredential.RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota


                var vault = new Windows.Security.Credentials.PasswordVault();

                vault.Remove(new Windows.Security.Credentials.PasswordCredential(
                    "classevivapcto", loginCredential.UserName.ToString(), loginCredential.Password.ToString()));

            }

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }

        }



        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            s = s.ToLower();
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
