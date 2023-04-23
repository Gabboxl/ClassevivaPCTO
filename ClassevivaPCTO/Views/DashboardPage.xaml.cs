using ClassevivaPCTO.Adapters;
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

        public async Task LoadOverviewCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingAgenda = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            string caldate = VariousUtils.ToApiDateTime(DateTime.Now.AddDays(-2));
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
                    //ListViewAbsencesDate.ItemsSource = overviewResult.Grades;
                    ListViewVotiDate.ItemsSource = overviewResult.Grades;

                    //order lessons by evtHPos
                    var orderedlessons = overviewResult.Lessons.OrderBy(x => x.evtHPos).ToList();

                    //remove duplicates based on lessonArg and authorname and increment evtDuration it it is a duplicate
                    foreach (var lesson in orderedlessons.ToList())
                    {
                        var duplicates = orderedlessons
                            .Where(
                                x =>
                                    x.lessonArg == lesson.lessonArg
                                    && x.authorName == lesson.authorName
                            )
                            .ToList();
                        if (duplicates.Count > 1)
                        {
                            lesson.evtDuration += duplicates[1].evtDuration;
                            orderedlessons.Remove(duplicates[1]);
                        }
                    }

                    //orderedlessons = orderedlessons.GroupBy(x => x.lessonArg).Select(x => x.First()).ToList();

                    ListViewLezioniDate.ItemsSource = orderedlessons
                        .Select(les => new LessonAdapter(les))
                        .ToList();

                    // Wrap each AgendaEvent object in an instance of AgendaEventAdapter and handle null case
                    var eventAdapters = overviewResult.AgendaEvents
                        ?.Select(evt => new AgendaEventAdapter(evt))
                        .ToList();

                    ListViewAgendaDate.ItemsSource = eventAdapters;

                    DashboardPageViewModel.IsLoadingAgenda = false;
                }
            );
        }

        public async Task CaricaRecentGradesCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingGrades = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];
            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), loginResult.token.ToString())
                .ConfigureAwait(false);
            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);
            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ListRecentGrades.ItemsSource = fiveMostRecent;

                    DashboardPageViewModel.IsLoadingGrades = false;
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

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), loginResult.token.ToString())
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

        public async Task CaricaNoticesCard()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    DashboardPageViewModel.IsLoadingNotices = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            var resultNotices = await apiWrapper
                .GetNotices(cardResult.usrId.ToString(), loginResult.token.ToString())
                .ConfigureAwait(false);

            //get only most recent 5 notices
            var fiveMostRecent = resultNotices.Notices
                .OrderByDescending(x => x.cntValidFrom)
                .Take(5);

            var noticesAdapters = fiveMostRecent?.Select(evt => new NoticeAdapter(evt)).ToList();

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ListRecentNotices.ItemsSource = noticesAdapters;

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
    }
}
