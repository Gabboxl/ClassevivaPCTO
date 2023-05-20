using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
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
                    //await UpdateCalendar();

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


        private async Task UpdateCalendar()
        {
            var displayedDays = TestCalendar.FindDescendants().OfType<CalendarViewDayItem>();

            //check if displayeddays descendants are CalendarViewDayItem

            foreach (var displayedDay in displayedDays)
            {

                    await ColorDay(displayedDay);

            }
        }


        private async void MyCalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            /*// Check if the day item is being added to the calendar
            if (args.Phase == 0)
            {

                    // Register for the next phase to set the background color
                    args.RegisterUpdateCallback(MyCalendarView_CalendarViewDayItemChanging);

            }
            else if (args.Phase == 1)
            {
                if (_calendarResult != null)
                {
                    ColorDay(args.Item);
                }
            }*/


            if (_calendarResult != null)
            {
                await ColorDay(args.Item);
            }
        }

        private Task ColorDay(CalendarViewDayItem calendarViewDayItem)
        {
            foreach (var calendarDay in _calendarResult.CalendarDays)
            {
                if (calendarViewDayItem.Date.Date == calendarDay.dayDate.Date)
                {
                    Debug.WriteLine(calendarViewDayItem.Date.Date + ", " + calendarDay.dayDate.Date);

                    if (calendarDay.dayStatus == DayStatus.SD)
                    {

                        calendarViewDayItem.Background = new SolidColorBrush(Colors.Teal);

                        foreach (var currentAbsenceEvent in _absencesResult.AbsenceEvents)
                        {
                            if (currentAbsenceEvent.evtDate.Date.Equals(calendarViewDayItem.Date.Date))
                            {

                                calendarViewDayItem.Background = CvUtils.GetColorFromAbsenceCode(currentAbsenceEvent.evtCode);

                                break;
                            }

                        }

                    }
                    else
                    {
                        calendarViewDayItem.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    break;
                }
                else
                {
                    calendarViewDayItem.Background = new SolidColorBrush(Colors.Transparent);
                }
            }

            return Task.CompletedTask;
        }

    }
}
