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
            if (CalendarAgenda.Date.HasValue)
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

                string apiDate = VariousUtils.ToApiDateTime(CalendarAgenda.Date.Value.Date);

                await Task.Run(async () =>
                {
                    await LoadData(apiDate);
                });
            }
        }

        private async Task LoadData(string apiDateToLoad)
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

            OverviewResult overviewResult = await apiWrapper.GetOverview(
                cardResult.usrId.ToString(),
                apiDateToLoad,
                apiDateToLoad,
                loginResult.token.ToString()
            );

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    //ListViewAbsencesDate.ItemsSource = overviewResult.Grades;
                    ListViewVotiDate.ItemsSource = overviewResult.Grades;

                    ListViewLezioniDate.ItemsSource = overviewResult.Lessons;

                    ListViewAgendaDate.ItemsSource = overviewResult.AgendaEvents;

                    AgendaViewModel.AreSourcesEmpty = (
                        overviewResult.AbsenceEvents.Count == 0
                        && overviewResult.Lessons.Count == 0
                        && overviewResult.Grades.Count == 0
                        && overviewResult.AgendaEvents.Count == 0
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
            string apiDate = VariousUtils.ToApiDateTime(CalendarAgenda.Date.Value.Date);

            await Task.Run(async () =>
            {
                await LoadData(apiDate);
            });
        }
    }
}
