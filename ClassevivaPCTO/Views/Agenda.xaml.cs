using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassevivaPCTO.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class Agenda : Page
    {
        public AgendaViewModel AgendaViewModel { get; } = new AgendaViewModel();

        private readonly IClassevivaAPI apiWrapper;

        public Agenda()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AgendaViewModel.IsLoadingAgenda = true;

            //listender for calendaragenda date change
            CalendarAgenda.DateChanged += CalendarAgenda_DateChanged;

            //imposto la data di oggi del picker, e aziono il listener per il cambiamento della data
            CalendarAgenda.Date = DateTime.Now;
        }

        private async void CalendarAgenda_DateChanged(
            CalendarDatePicker sender,
            CalendarDatePickerDateChangedEventArgs args
        )
        {
            var agendaSelectedDate = CalendarAgenda.Date;

            if (agendaSelectedDate.HasValue)
            {
                //if the date is today, then the button to go to today is disabled
                if (CalendarAgenda.Date.Value.Date == DateTime.Now.Date)
                {
                    ButtonToday.IsChecked = true;
                    //ButtonToday.IsEnabled = false;
                }
                else
                {
                    ButtonToday.IsChecked = false;
                    //ButtonToday.IsEnabled = true;
                }

                await Task.Run(async () =>
                {
                    await LoadData(agendaSelectedDate.Value.Date);
                });
            }
        }

        private async Task LoadData(DateTime dateToLoad)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    AgendaViewModel.IsLoadingAgenda = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            string apiDate = VariousUtils.ToApiDateTime(dateToLoad);

            OverviewResult overviewResult = await apiWrapper.GetOverview(
                cardResult.usrId.ToString(),
                apiDate,
                apiDate,
                loginResult.token.ToString()
            );

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    //ListViewAbsencesDate.ItemsSource = overviewResult.Grades;

                    var filteredGrades = overviewResult.Grades
                        .Where(grade => grade.evtDate == dateToLoad)
                        .ToList();

                    //grades
                    ListViewVotiDate.ItemsSource = filteredGrades;

                    //lessons
                    ListViewLezioniDate.ItemsSource = overviewResult.Lessons;

                    //filter agenda events if the selected date is between the start and end date of the event
                    var filteredAgendaEvents = overviewResult.AgendaEvents
                        .Where(
                            agendaEvent =>
                                agendaEvent.evtDatetimeBegin <= dateToLoad && agendaEvent.evtDatetimeEnd >= dateToLoad
                        )
                        .ToList();

                    //agenda
                    ListViewAgendaDate.ItemsSource = filteredAgendaEvents;

                    AgendaViewModel.AreSourcesEmpty = (
                        overviewResult.AbsenceEvents.Count == 0
                        && overviewResult.Lessons.Count == 0
                        && filteredGrades.Count == 0
                        && filteredAgendaEvents.Count == 0
                    //need to check notes
                    );

                    AgendaViewModel.IsLoadingAgenda = false;
                }
            );
        }

        private void ButtonToday_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CalendarAgenda.Date = DateTime.Now;
        }

        private void ButtonYesterday_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CalendarAgenda.Date = CalendarAgenda.Date.Value.AddDays(-1);
        }

        private void ButtonTomorrow_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //add one day to the calendaragenda date
            CalendarAgenda.Date = CalendarAgenda.Date.Value.AddDays(1);
        }

        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var agendaSelectedDate = CalendarAgenda.Date;

            await Task.Run(async () =>
            {
                await LoadData(agendaSelectedDate.Value.Date);
            });
        }
    }
}
