using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.DataModels;

namespace ClassevivaPCTO.Views
{
    public sealed partial class DashboardPage : Page
    {
        private readonly IClassevivaAPI apiWrapper;

        public DashboardPageViewModel DashboardPageViewModel { get; } =
            new DashboardPageViewModel();

        public DashboardPage()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            TextBenvenuto.Text = "Dashboard di " + VariousUtils.ToTitleCase(cardResult.firstName);

            /*

            JsonConvert.DefaultSettings =
                       () => new JsonSerializerSettings()
                       {
                           Converters = { new CustomIntConverter() }
                       };

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new CustomIntConverter());

            */

            await LoadEverything();
        }

        private async Task LoadOverviewCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingAgenda = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            string caldate = VariousUtils.ToApiDateTime(DateTime.Now);
            OverviewResult overviewResult = await apiWrapper.GetOverview(
                cardResult.usrId.ToString(),
                caldate,
                caldate,
                loginResult.token.ToString()
            );


            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    var overviewData = new OverviewDataModel
                    {
                        OverviewData = overviewResult,
                        FilterDate = DateTime.Now
                    };

                    OverviewListView.ItemsSource = overviewData;

                    DashboardPageViewModel.IsLoadingAgenda = false;
                }
            );
        }

        private async Task CaricaRecentGradesCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingGrades = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;
            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), loginResult.token)
                .ConfigureAwait(false);

            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ListRecentGrades.ItemsSource = fiveMostRecent.ToList();

                    DashboardPageViewModel.IsLoadingGrades = false;
                }
            );
        }

        private async Task CaricaMediaCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingMedia = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), loginResult.token)
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

        private async Task CaricaNoticesCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingNotices = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            var resultNotices = await apiWrapper
                .GetNotices(cardResult.usrId.ToString(), loginResult.token.ToString())
                .ConfigureAwait(false);

            //get only most recent 5 notices and filter by active status
            var fiveMostRecent = resultNotices.Notices
                .Where(x => x.cntValidInRange)
                .OrderByDescending(x => x.cntValidFrom)
                .Take(5);

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ListRecentNotices.ItemsSource = fiveMostRecent.ToList();

                    DashboardPageViewModel.IsLoadingNotices = false;
                }
            );
        }

        private async Task LoadEverything()
        {
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

            await Task.Run(async () =>
            {
                await CaricaNoticesCard();
            });
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
            await LoadEverything();
        }

        private void ButtonApriBacheca_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.Bacheca));
        }
    }
}
