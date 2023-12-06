using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Services;
using CommunityToolkit.WinUI;

namespace ClassevivaPCTO.Views
{
    public sealed partial class AssenzePage : Page
    {
        public AssenzeViewModel AssenzeViewModel { get; } = new();

        private readonly IClassevivaAPI apiWrapper;

        private CalendarResult _apiCalendarResult;

        private AbsencesResult? _absencesResult;

        public AssenzePage()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);

            //set the min and max date of the calendaragenda
            var agedaDates = VariousUtils.GetAgendaStartEndDates();

            ColoredCalendarView.MinDate = agedaDates.startDate;
            ColoredCalendarView.MaxDate = agedaDates.endDate;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AssenzeViewModel.IsLoadingAssenze = true;


            await Task.Run(async () => { await LoadData(); });
        }

        private async Task LoadData()
        {
            try

            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { AssenzeViewModel.IsLoadingAssenze = true; }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

                AbsencesResult absencesResult = await apiWrapper.GetAbsences(
                    cardResult.usrId.ToString()
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
                    cardResult.usrId.ToString()
                );

                _apiCalendarResult = calendarResult;

                //update UI on UI thread
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        AbsencesToJustifyListView.ItemsSource = notJustifiedAbsences.Concat(justifiedAbsences).ToList();
                        //AbsencesJustifiedListView.ItemsSource = justifiedAbsences;

                        await UpdateCalendar();

                        //select the current day of the calendar
                        ColoredCalendarView.SetDisplayDate(DateTime.Now.Date);
                    }
                );
            }
            finally
            {
                {
                    await CoreApplication.MainView.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        async () => { AssenzeViewModel.IsLoadingAssenze = false; }
                    );
                }
            }
        }

        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(async () => { await LoadData(); });
        }

        private async Task UpdateCalendar()
        {
            var displayedDays = ColoredCalendarView.FindDescendants().OfType<CalendarViewDayItem>();

            //check if displayeddays descendants are CalendarViewDayItem

            foreach (var displayedDay in displayedDays)
            {
                await ColorDay(displayedDay);
            }
        }

        private async void MyCalendarView_CalendarViewDayItemChanging(CalendarView sender,
            CalendarViewDayItemChangingEventArgs args)
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


            if (_apiCalendarResult != null)
            {
                await ColorDay(args.Item);
            }
        }

        private Task ColorDay(CalendarViewDayItem calendarViewDayItem)
        {
            foreach (var apiCalendarDay in _apiCalendarResult.CalendarDays)
            {
                if (calendarViewDayItem.Date.Date == apiCalendarDay.dayDate.Date &&
                    calendarViewDayItem.Date.Date <=
                    DateTime.Now.Date) //we make sure that the date is not in the future
                {
                    if (apiCalendarDay.dayStatus == DayStatus.SD)
                    {
                        calendarViewDayItem.Background =
                            new SolidColorBrush(PaletteSelectorService.PaletteClass.ColorGreen);

                        foreach (var currentAbsenceEvent in _absencesResult.AbsenceEvents.Where(currentAbsenceEvent =>
                                     currentAbsenceEvent.evtDate.Date.Equals(calendarViewDayItem.Date.Date)))
                        {
                            calendarViewDayItem.Background =
                                CvUtils.GetColorFromAbsenceCode(currentAbsenceEvent.evtCode);

                            break;
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

        private void TodayButton_OnClick(object sender, RoutedEventArgs e)
        {
            //select the current day of the calendar
            ColoredCalendarView.SetDisplayDate(DateTime.Now.Date);
        }
    }
}