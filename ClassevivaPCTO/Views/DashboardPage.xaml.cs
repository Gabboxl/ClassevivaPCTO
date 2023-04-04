using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Polly;
using Polly.Retry;
using System.Linq.Expressions;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;

namespace ClassevivaPCTO.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {

        private readonly IClassevivaAPI apiClient;

        public DashboardPageViewModel DashboardPageViewModel { get; } = new DashboardPageViewModel();



        public DashboardPage()
        {
            this.InitializeComponent();

            App app = (App)App.Current;

            apiClient = App.GetService<IClassevivaAPI>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


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


            var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);
            var result1 = await api.GetGrades(cardResult.usrId.ToString(), loginResult.Token.ToString());



            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);

            ListRecentGrades.ItemsSource = fiveMostRecent;


            //todoo
            ListViewAbsencesDate.ItemsSource = result1.Grades;
            ListViewVotiDate.ItemsSource = result1.Grades;
            ListViewAgendaDate.ItemsSource = result1.Grades;


            await CaricaMediaCard();

        }



        public async Task CaricaMediaCard()
        {
            DashboardPageViewModel.IsLoadingMedia = true;


            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            //var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);

            //apiClient = Container.GetService<IClassevivaAPI>();

            
            var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);

            var wrapper = new ApiWrapper<IClassevivaAPI>(apiClient);


            var result1 = await wrapper.CallApi(x => x.GetGrades(cardResult.usrId.ToString(), loginResult.Token.ToString())); //loginResult.Token.ToString()


            // Calcoliamo la media dei voti
            float media = CalcolaMedia(result1.Grades);

            TextBlockMedia.Foreground = (Brush)new GradeToColorConverter().Convert(media, null, null, null);

            // Stampiamo la media dei voti
            TextBlockMedia.Text = media.ToString("0.00");
            TextBlockMedia.Visibility = Visibility.Visible;

            DashboardPageViewModel.IsLoadingMedia = false;
        }


        public class ApiWrapper<T> where T : class
        {
            private readonly T _api;

            public ApiWrapper(T api)
            {
                _api = api;
            }

            public async Task<TResult> CallApi<TResult>(Func<T, Task<TResult>> apiCall)
            {

                DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

                var policy = Policy
                    .Handle<ApiException>()
                    
                    .RetryAsync(96,
                            async (ex, count) =>
                            {
                                Debug.WriteLine("Retry {0} times", count);

                                var loginCredentials = new CredUtils().GetCredentialFromLocker();

                                if (loginCredentials != null)
                                {
                                    
                                        await App.Window.DispatcherQueue.EnqueueAsync(async () =>
                                        {
                                            ContentDialog dialog = new ContentDialog();
                                            dialog.Title = "Errore";
                                            dialog.PrimaryButtonText = "OK";
                                            dialog.DefaultButton = ContentDialogButton.Primary;
                                            dialog.Content = "Errore durante il login. Controlla il nome utente e la password. \n Errore:" ;
                                            dialog.XamlRoot = App.Window.Content.XamlRoot;

                                            try
                                            {
                                                var result = await dialog.ShowAsync(); //attenzione, se togli l'await l'esecuzione dei retry continua fino alla fine del limite massimo
                                            }
                                            catch (Exception e)
                                            {
                                                System.Console.WriteLine(e.ToString());
                                            }
                                        });

                                 
                                }

                                        
                            }
                                );

                return await policy.ExecuteAsync(async () => await apiCall(_api));
            }




        }




        static float CalcolaMedia(List<Grade> voti)
        {
            float somma = 0;
            float numVoti = 0;

            foreach (Grade voto in voti)
            {
                float? valoreDaSommare = VariousUtils.GradeToFloat(voto);

                if (valoreDaSommare != null)
                {
                    somma += (float)valoreDaSommare;

                    numVoti++;
                }
            }

            return somma / numVoti;
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
