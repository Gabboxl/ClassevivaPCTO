using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Wrap;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ClassevivaPCTO.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        private readonly IClassevivaAPI apiClient;
        private readonly ApiPolicyWrapper<IClassevivaAPI> apiWrapper;

        private readonly IClassevivaAPI apiWrapper2;

        public DashboardPageViewModel DashboardPageViewModel { get; } =
            new DashboardPageViewModel();

        public DashboardPage()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            apiClient = app.Container.GetService<IClassevivaAPI>();

            //apiWrapper = new ApiPolicyWrapper<IClassevivaAPI>(apiClient);

            apiWrapper2 = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        //the app crashed with the error "Access is denied" because that class wasn't marked as "public"

        public class PoliciesDispatchProxy<T> : DispatchProxy
            where T : class, IClassevivaAPI
        {
            private T Target { get; set; }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                var retryPolicy = Policy
                    .Handle<AggregateException>()
                    .RetryAsync(
                        3,
                        async (exception, retryCount, context) =>
                        {
                            if(exception.InnerException is ApiException apiException)
                            {
                                if(apiException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                                {
                                    //refresh token
                                    //await apiClient.RefreshTokenAsync();
                                    Debug.WriteLine("tonyeffe ok");
                                }
                            }

                            //Task.Delay(3000);

                            //run on ui thread
                            
                                //show dialog
                                ContentDialog dialog = new ContentDialog();
                                dialog.Title = "Sessione scaduta";
                                dialog.PrimaryButtonText = "OK";
                                dialog.DefaultButton = ContentDialogButton.Primary;
                                //dialog.XamlRoot = Window.Current.Content.XamlRoot;
                                dialog.Content =
                                    "Aggiornamento dei dati di login in corso...";
                                try
                                {
                                    var result = dialog.ShowAsync();
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.ToString());
                                }

                            
                        


                        }
                    );

                var fallback = Policy<object>
                    .Handle<Exception>()
                    .FallbackAsync(async ct =>
                    {
                        //if after the retries another exception occurs, then we let the call flow go ahead
                        return targetMethod.Invoke(Target, args);
                    });

                AsyncPolicyWrap<object> combinedpolicy = fallback.WrapAsync(retryPolicy);

                return combinedpolicy
                    .ExecuteAsync(async () =>
                    {
                        var lol = (targetMethod.Invoke(Target, args));

                        if (lol is Task task)
                        {
                            task.Wait(); //we wait for the result of the task, so we catch the exceptions if there are any
                        }

                        return lol; //if no exception occur then we return the result of the method call
                    })
                    .Result;
            }

            public static T CreateProxy(T target)
            {
                var proxy = Create<T, PoliciesDispatchProxy<T>>() as PoliciesDispatchProxy<T>;
                proxy.Target = target;
                return proxy as T;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            TextBenvenuto.Text =
                "Dashboard di " + VariousUtils.UppercaseFirst(cardResult.firstName);

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
            var result1 = await api.GetGrades(
                cardResult.usrId.ToString(),
                loginResult.Token.ToString()
            );

            //var result1 = await apiWrapper.CallApi(x => x.GetGrades(cardResult.usrId.ToString(), loginResult.Token.ToString()));




            //api.GetGrades(cardResult.usrId.ToString(), "tony");



            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);

            ListRecentGrades.ItemsSource = fiveMostRecent;

            //todoo
            ListViewAbsencesDate.ItemsSource = result1.Grades;
            ListViewVotiDate.ItemsSource = result1.Grades;
            ListViewAgendaDate.ItemsSource = result1.Grades;

            CaricaMediaCard();
        }

        public async void CaricaMediaCard()
        {
            DashboardPageViewModel.IsLoadingMedia = true;

            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            //var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);

            //apiClient = Container.GetService<IClassevivaAPI>();


            var result1 = await apiWrapper2.GetGrades(cardResult.usrId.ToString(), "asd");

            // Calcoliamo la media dei voti
            float media = CalcolaMedia(result1.Grades);

            TextBlockMedia.Foreground = (Brush)
                new GradeToColorConverter().Convert(media, null, null, null);

            // Stampiamo la media dei voti
            TextBlockMedia.Text = media.ToString("0.00");
            TextBlockMedia.Visibility = Visibility.Visible;

            DashboardPageViewModel.IsLoadingMedia = false;
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
