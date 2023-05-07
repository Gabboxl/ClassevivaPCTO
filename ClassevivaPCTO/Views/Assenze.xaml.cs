using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassevivaPCTO.Views
{
    public sealed partial class Assenze : Page
    {
        public AssenzeViewModel AssenzeViewModel { get; } = new AssenzeViewModel();

        private readonly IClassevivaAPI apiWrapper;

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
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];


            AbsencesResult noticeboardResult = await apiWrapper.GetAbsences(
                cardResult.usrId.ToString(),
                loginResult.token.ToString()
            );


            //create list based on isjustified bool value
            var justifiedAbsences = noticeboardResult.AbsenceEvents
                .OrderByDescending(n => n.evtDate)
                .Where(n => n.isJustified)
                .ToList();

            //not justified absences
            var notJustifiedAbsences = noticeboardResult.AbsenceEvents
                .OrderByDescending(n => n.evtDate)
                .Where(n => !n.isJustified)
                .ToList();


            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    AbsencesTojustifyListView.ItemsSource = notJustifiedAbsences;
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
    }
}
