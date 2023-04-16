using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ClassevivaPCTO.Views
{
    public sealed partial class DashboardPage : Page
    {
        private readonly IClassevivaAPI apiClient;

        private readonly IClassevivaAPI apiWrapper;

        public DashboardPageViewModel DashboardPageViewModel { get; } =
            new DashboardPageViewModel();

        public DashboardPage()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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

            await Task.Run(async () =>
            {
                await LoadOverviewCard();
            });

            await Task.Run(async () =>
            {
                await CaricaRecentGradesCard();
            });

            //run in a background thread otherwise the UI thread gets stuck when displaying a dialog
            await Task.Run(async () =>
            {
                await CaricaMediaCard();
            });
        }

        public async Task LoadOverviewCard()
        {
            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];



            string caldate = VariousUtils.ToApiDateTime(DateTime.Now.AddDays(-2));
            OverviewResult overviewResult = await apiWrapper.GetOverview(cardResult.usrId.ToString(), caldate, caldate, loginResult.Token.ToString());


            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {   
                    //ListViewAbsencesDate.ItemsSource = overviewResult.Grades;
                    //ListViewVotiDate.ItemsSource = overviewResult.Grades;
                    //ListViewLezioniDate.ItemsSource = overviewResult.Grades;

                    // Wrap each AgendaEvent object in an instance of AgendaEventAdapter
                    var eventAdapters = overviewResult.AgendaEvents.Select(ev => new AgendaEventAdapter(ev)).ToList();

                    ListViewAgendaDate.ItemsSource = eventAdapters;
                }
            );
        }











        public async Task CaricaRecentGradesCard()
        {
            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];
            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), loginResult.Token.ToString())
                .ConfigureAwait(false);
            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);
            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ListRecentGrades.ItemsSource = fiveMostRecent;
                }
            );
        }

        public async Task CaricaMediaCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingMedia = true;
                }
            );

            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), loginResult.Token.ToString())
                .ConfigureAwait(false);

            // Calcoliamo la media dei voti
            float media = VariousUtils.CalcolaMedia(result1.Grades);

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    TextBlockMedia.Foreground = (Brush)
                        new GradeToColorConverter().Convert(media, null, null, null);

                    // Stampiamo la media dei voti
                    TextBlockMedia.Text = media.ToString("0.00");
                    TextBlockMedia.Visibility = Visibility.Visible;

                    DashboardPageViewModel.IsLoadingMedia = false;
                }
            );
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

        private async void AggiornaButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardPageViewModel.IsLoadingMedia = true;

            await Task.Run(async () =>
            {
                await LoadOverviewCard();
            });

            await Task.Run(async () =>
            {
                await CaricaRecentGradesCard();
            });


            await Task.Run(async () =>
            {
                await CaricaMediaCard();
            });
        }
    }
}
