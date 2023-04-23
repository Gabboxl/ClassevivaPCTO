using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Refit;
using System;
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

        public Agenda()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AgendaViewModel.IsLoadingAgenda = true;

            //imposto la data di oggi del picker
            CalendarAgenda.Date = DateTime.Now;

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);

            //string fixedId = new CvUtils().GetCode(loginResult.Ident);

            string caldate = VariousUtils.ToApiDateTime(CalendarAgenda.Date.Value.DateTime);

            OverviewResult overviewResult = await api.GetOverview(
                cardResult.usrId.ToString(),
                caldate,
                caldate,
                loginResult.token.ToString()
            );

            AgendaViewModel.IsLoadingAgenda = false;
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
    }
}
