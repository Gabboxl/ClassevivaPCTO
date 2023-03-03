using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Utils;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;





// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassevivaPCTO
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public ObservableCollection<Grade> Voti { get; } = new ObservableCollection<Grade>();

        public DashboardPage()
        {
            this.InitializeComponent();


            //titolo title bar
            AppTitleTextBlock.Text = "Dashboard - " + AppInfo.Current.DisplayInfo.DisplayName;
            Window.Current.SetTitleBar(AppTitleBar);




        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoginResult parameters = (LoginResult)e.Parameter;

            TextBenvenuto.Text = "Dashboard di " + UppercaseFirst(parameters.FirstName) + " " + UppercaseFirst(parameters.LastName);
           


            /*

            JsonConvert.DefaultSettings =
                       () => new JsonSerializerSettings()
                       {
                           Converters = { new CustomIntConverter() }
                       };

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new CustomIntConverter());

            */


            var api = RestService.For<IClassevivaAPI>("https://web.spaggiari.eu/rest/v1");

            string fixedId = new CvUtils().GetCode(parameters.Ident);

            var result1 = await api.GetGrades(fixedId, parameters.Token.ToString());

            Voti.Concat(result1.Grades);

            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);
            
            Listtest.ItemsSource = fiveMostRecent;

            //textDati.Text = result1.Events.Count().ToString();


            PersonPictureDashboard.DisplayName = UppercaseFirst(parameters.FirstName) + " " + UppercaseFirst(parameters.LastName);


            List<float?> voti = new List<float?>();

            foreach(Grade voto in result1.Grades)
            {
                voti.Add(voto.decimalValue);
            }



            // Simuliamo l'acquisizione dei voti dal server Classeviva

          
                // Calcoliamo la media dei voti
                float media = CalcolaMedia(voti);

            TextBlockMedia.Foreground = (Brush)new GradeToColorConverter().Convert(media,null,null,null);

          



            // Stampiamo la media dei voti
            TextBlockMedia.Text = media.ToString("0.00");



        }

        static float CalcolaMedia(List<float?> voti)
        {
            float somma = 0;

            foreach (float voto in voti)
            {
                somma += voto;
            }

            return (float)somma / voti.Count;

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


        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DettaglioVoti));
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
