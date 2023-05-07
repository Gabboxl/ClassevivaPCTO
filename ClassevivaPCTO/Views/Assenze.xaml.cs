using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
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

            /*
            await Task.Run(async () =>
            {
                await LoadData();
            });*/
        }

        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                //await LoadData();
            });
        }
    }
}
