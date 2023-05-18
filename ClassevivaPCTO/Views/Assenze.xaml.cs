using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI;

namespace ClassevivaPCTO.Views
{
    public sealed partial class Assenze : Page
    {
        public AssenzeViewModel AssenzeViewModel { get; } = new AssenzeViewModel();

        private readonly IClassevivaAPI apiWrapper;

        private CalendarResult _calendarResult;

        private AbsencesResult _absencesResult;

        public Assenze()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AssenzeViewModel.IsLoadingAssenze = true;

            
            await Task.Run(async () =>
            {
                await LoadData();
            });
        }


        private async Task LoadData()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    AssenzeViewModel.IsLoadingAssenze = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;


            AbsencesResult absencesResult = await apiWrapper.GetAbsences(
                cardResult.usrId.ToString(),
                loginResult.token
            );

            _absencesResult = absencesResult;


            //create list based on isjustified bool value
            var justifiedAbsences = absencesResult.AbsenceEvents
                .OrderByDescending(n => n.evtDate)
                .Where(n => n.isJustified)
                .ToList();

            //not justified absences
            var notJustifiedAbsences = absencesResult.AbsenceEvents
                .OrderByDescending(n => n.evtDate)
                .Where(n => !n.isJustified)
                .ToList();



            //calendar thigs
            CalendarResult calendarResult = await apiWrapper.GetCalendar(
                               cardResult.usrId.ToString(),
                                              loginResult.token
                                          );

            _calendarResult = calendarResult;

            




            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    UpdateCalendar();

                    AbsencesToJustifyListView.ItemsSource = notJustifiedAbsences;
                    AbsencesJustifiedListView.ItemsSource = justifiedAbsences;

                    AssenzeViewModel.IsLoadingAssenze = false;
                }
            );

        }




        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                await LoadData();
            });
        }


        private void UpdateCalendar()
        {
            var displayedDays = TestCalendar.FindDescendants().OfType<CalendarViewDayItem>();

            //check if displayeddays descendants are CalendarViewDayItem

            foreach (var displayedDay in displayedDays)
            {

                foreach (var calendarDay in _calendarResult.CalendarDays)
                {
                    if (displayedDay.Date.Date == calendarDay.dayDate.Date)
                    {
                        if (calendarDay.dayStatus == DayStatus.SD)
                        {

                            foreach (var priv in _absencesResult.AbsenceEvents)
                            {
                                if (priv.evtDate.Date.Equals(displayedDay.Date.Date))
                                {

                                    displayedDay.Background = CvUtils.GetColorFromAbsenceCode(priv.evtCode);

                                    break;
                                }

                            }

                            displayedDay.Background = new SolidColorBrush(Colors.Teal);
                        }
                        else
                        {
                            displayedDay.Background = new SolidColorBrush(Colors.Transparent);
                        }
                    }
                }

            }
        }


        private void MyCalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            // Check if the day item is being added to the calendar
            if (args.Phase == 0)
            {

                    // Register for the next phase to set the background color
                    args.RegisterUpdateCallback(MyCalendarView_CalendarViewDayItemChanging);

            }
            else if (args.Phase == 1)
            {
                // Check if the day should be colored in teal
                if (ShouldColorDayInTeal(args.Item.Date))
                {
                    // Set the background color of the day item to teal
                    args.Item.Background = new SolidColorBrush(Colors.Teal);
                }
                else
                {
                    // Explicitly set the background color for odd days
                    args.Item.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        private bool ShouldColorDayInTeal(DateTimeOffset date)
        {
            // Add your custom logic to determine if the day should be colored in teal
            // For example, you can check if the day is a specific date or part of a list of dates
            return date.Day % 2 == 0; // This example colors all even days in teal
        }

    }
}
