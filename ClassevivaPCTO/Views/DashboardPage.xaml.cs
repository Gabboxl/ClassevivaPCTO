using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.Storage;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Controls;
using ClassevivaPCTO.DataModels;
using ClassevivaPCTO.Helpers;

namespace ClassevivaPCTO.Views
{
    public sealed partial class DashboardPage : CustomAppPage
    {
        private readonly IClassevivaAPI apiWrapper;

        public DashboardPageViewModel DashboardPageViewModel { get; } = new();

        public DashboardPage()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

            this.ListRecentNotices.OnShouldUpdate += OnShouldUpdate;

            TextTitolo.Text = string.Format("DashboardTitleText".GetLocalizedStr(),
                VariousUtils.ToTitleCase(cardResult.firstName));

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
            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingAgenda = true; }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

                string caldate = VariousUtils.ToApiDateTime(DateTime.Now);
                OverviewResult overviewResult = await apiWrapper.GetOverview(
                    cardResult.usrId.ToString(),
                    caldate,
                    caldate);

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
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingAgenda = false; }
                );
            }
        }

        private async Task CaricaRecentGradesCard()
        {
            int maxGradeRecords = await ApplicationData.Current.LocalSettings.ReadAsync<int?>("MaxGradesWidgetRecord") ?? 3;

            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingGrades = true; }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

                var result1 = await apiWrapper
                    .GetGrades(cardResult.usrId.ToString())
                    .ConfigureAwait(false);

                var fiveMostRecent = result1.Grades
                    .OrderByDescending(x => x.evtDate)
                    .Take(maxGradeRecords);

                //update UI on UI thread
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { ListRecentGrades.ItemsSource = fiveMostRecent.ToList(); }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingGrades = false; }
                );
            }
        }

        private async Task CaricaMediaCard()
        {
            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingMedia = true; }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

                Grades2Result? result1 = await apiWrapper
                    .GetGrades(cardResult.usrId.ToString())
                    .ConfigureAwait(false);

                // Calcoliamo la media dei voti
                float media = VariousUtils.CalcolaMedia(result1?.Grades);

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
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingMedia = false; }
                );
            }
        }

        private async Task CaricaNoticesCard()
        {

            int maxNoticesRecords = await ApplicationData.Current.LocalSettings.ReadAsync<int?>("MaxNoticesWidgetRecord") ?? 3;

            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingNotices = true; }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

                var resultNotices = await apiWrapper
                    .GetNotices(cardResult.usrId.ToString())
                    .ConfigureAwait(false);

                //get only most recent 5 notices and filter by active status
                var fiveMostRecent = resultNotices.Notices
                    .Where(x => x.cntValidInRange)
                    .OrderByDescending(x => x.cntValidFrom)
                    .Take(maxNoticesRecords);

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
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { DashboardPageViewModel.IsLoadingNotices = false; }
                );
            }
        }

        private async Task LoadEverything()
        {
            //execute everything in parallel
            var task1 = Task.Run(async () => { await LoadOverviewCard(); });

            var task2 = Task.Run(async () => { await CaricaRecentGradesCard(); });

            var task3 = Task.Run(async () => { await CaricaMediaCard(); });

            var task4 = Task.Run(async () => { await CaricaNoticesCard(); });

            //wait for all tasks to complete (also useful to get and rethrow exceptions that happened inside the tasks)
            Task taskall = Task.WhenAll(task1, task2, task3, task4);
            
            try
            {
                await taskall;
            }
            catch (Exception)
            {
                if (taskall.Exception != null)
                {
                    throw taskall.Exception;
                }
            }
        }

        private void OnShouldUpdate(object sender, EventArgs args)
        {
            AggiornaAction();
        }

        private void HyperlinkButton_Click_Valutazioni(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(typeof(Views.DettaglioVoti), null);
            NavigationService.Navigate(typeof(ValutazioniPage));
        }

        private async void HyperlinkButton_Click_Agenda(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(AgendaPage));
        }

        public override void AggiornaAction()
        {
            Task.Run(async () => { await LoadEverything(); });
        }

        private void ButtonApriBacheca_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(BachecaPage));
        }
    }
}