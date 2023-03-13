using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;





// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassevivaPCTO
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        //public ObservableCollection<Grade> Voti { get; } = new ObservableCollection<Grade>();

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

            //LoginResult parameters = (LoginResult)e.Parameter;

            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;

            TextBenvenuto.Text = "Dashboard di " + VariousUtils.UppercaseFirst(loginResult.FirstName) + " " + VariousUtils.UppercaseFirst(loginResult.LastName);
           
            


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

            string fixedId = new CvUtils().GetCode(loginResult.Ident);

            var result1 = await api.GetGrades(fixedId, loginResult.Token.ToString());



            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);
            
            Listtest.ItemsSource = fiveMostRecent;


            //imposto la data di oggi del picker
            CalendarAgenda.Date = DateTime.Now;

            //todoo
            ListViewAbsencesDate.ItemsSource = result1.Grades;
            ListViewVotiDate.ItemsSource = result1.Grades;
            ListViewAgendaDate.ItemsSource = result1.Grades;

            //textDati.Text = result1.Events.Count().ToString();


            PersonPictureDashboard.DisplayName = VariousUtils.UppercaseFirst(loginResult.FirstName) + " " + VariousUtils.UppercaseFirst(loginResult.LastName);

    
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
            TextBlockMedia.Visibility = Visibility.Visible;
            ProgressRingMedia.Visibility = Visibility.Collapsed;


        }

        static float CalcolaMedia(List<float?> voti)
        {
            float? somma = 0;

            foreach (float? voto in voti)
            {
                if (voto != null) {
                    somma += voto;
                }
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




        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DettaglioVoti));
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;

            var api = RestService.For<IClassevivaAPI>("https://web.spaggiari.eu/rest/v1");

            string fixedId = new CvUtils().GetCode(loginResult.Ident);

            var result1 = await api.GetGrades(fixedId, loginResult.Token.ToString());


            //aaaa

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });

            savePicker.SuggestedFileName = "New Document";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {

                Windows.Storage.CachedFileManager.DeferUpdates(file);

                var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(result1.Grades, Formatting.Indented);

                        await Windows.Storage.FileIO.WriteTextAsync(file, serialized);

                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "salvato";
                    dialog.PrimaryButtonText = "OK";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    dialog.Content = "ok salvato";
                        var result = await dialog.ShowAsync();
                    }
                else
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "rip";
                    dialog.PrimaryButtonText = "OK";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    dialog.Content = "rip";
                    var result = await dialog.ShowAsync();
                }
            }
            else
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "nop";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = "nop";
                var result = await dialog.ShowAsync();
            }

        }
    }
}
