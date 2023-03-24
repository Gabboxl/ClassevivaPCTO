﻿using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Services;
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

namespace ClassevivaPCTO.Views
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
            //AppTitleTextBlock.Text = "Dashboard - " + AppInfo.Current.DisplayInfo.DisplayName;
            //Window.Current.SetTitleBar(AppTitleBar);




        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //LoginResult parameters = (LoginResult)e.Parameter;

            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            TextBenvenuto.Text = "Dashboard di " + VariousUtils.UppercaseFirst(cardResult.firstName);
           
            


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

            //string fixedId = new CvUtils().GetCode(loginResult.Ident);

            var result1 = await api.GetGrades(cardResult.usrId.ToString(), loginResult.Token.ToString());



            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);
            
            Listtest.ItemsSource = fiveMostRecent;


            //todoo
            ListViewAbsencesDate.ItemsSource = result1.Grades;
            ListViewVotiDate.ItemsSource = result1.Grades;
            ListViewAgendaDate.ItemsSource = result1.Grades;

            //textDati.Text = result1.Events.Count().ToString();





            // Simuliamo l'acquisizione dei voti dal server Classeviva

          
                // Calcoliamo la media dei voti
                float media = CalcolaMedia(result1.Grades);

            TextBlockMedia.Foreground = (Brush)new GradeToColorConverter().Convert(media,null,null,null);



            // Stampiamo la media dei voti
            TextBlockMedia.Text = media.ToString("0.00");
            TextBlockMedia.Visibility = Visibility.Visible;
            ProgressRingMedia.Visibility = Visibility.Collapsed;


        }

        static float CalcolaMedia(List<Grade> voti)
        {
            float? somma = 0;

            foreach (Grade voto in voti)
            {
                if (voto != null) {
                    somma += VariousUtils.GradeToInt(voto);
                }
            }

            return (float)somma / voti.Count;

        }
    


        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(typeof(Views.DettaglioVoti), null);
            NavigationService.Navigate(typeof(Views.DettaglioVoti));
        }


        private async void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.Agenda));
        }
    }
}
