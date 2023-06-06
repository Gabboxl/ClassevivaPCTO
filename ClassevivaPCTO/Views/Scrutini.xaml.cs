using System;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassevivaPCTO.Views
{
    public sealed partial class Scrutini : Page
    {
        public ScrutiniViewModel ScrutiniViewModel { get; } = new ScrutiniViewModel();

        private readonly IClassevivaAPI apiWrapper;

        public Scrutini()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ScrutiniViewModel.IsLoadingScrutini = true;

            CheckboxEliminati.Checked += async (sender, args) => { await Task.Run(async () => { await LoadData(); }); };
            CheckboxEliminati.Unchecked +=
                async (sender, args) => { await Task.Run(async () => { await LoadData(); }); };


            await Task.Run(async () => { await LoadData(); });
        }


        private async Task LoadData()
        {
            bool showDeletedDocuments = false;

            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ScrutiniViewModel.IsLoadingScrutini = true;

                    if (CheckboxEliminati.IsChecked != null) showDeletedDocuments = CheckboxEliminati.IsChecked.Value;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.GetViewModel().LoginResult;
            Card cardResult = ViewModelHolder.GetViewModel().SingleCardResult;


            ScrutiniDocumentsResult scrutiniDocumentsResult = await apiWrapper.GetScrutiniDocuments(
                cardResult.usrId.ToString(),
                loginResult.token
            );


            foreach (ScrutiniDocument document in scrutiniDocumentsResult.Documents)
            {
                ScrutiniCheckResult scrutiniCheckResult = await apiWrapper.CheckScrutinioDocument(
                    cardResult.usrId.ToString(),
                    document.hash,
                    loginResult.token
                );

                document.checkStatus = scrutiniCheckResult.document;
            }


            //we take only available documents if the checkbox isnt checked, after we have checked them via the API
            if (!showDeletedDocuments)
            {
                scrutiniDocumentsResult.Documents =
                    scrutiniDocumentsResult.Documents.Where(d => d.checkStatus.available).ToList();
            }


            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ScrutiniListView.ItemsSource = scrutiniDocumentsResult;

                    ScrutiniViewModel.IsLoadingScrutini = false;
                }
            );
        }


        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(async () => { await LoadData(); });
        }
    }
}