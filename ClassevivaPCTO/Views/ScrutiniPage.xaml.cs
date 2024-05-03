using System;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Controls;

namespace ClassevivaPCTO.Views
{
    public sealed partial class ScrutiniPage : CustomAppPage
    {
        public ScrutiniViewModel ScrutiniViewModel { get; } = new();

        private readonly IClassevivaAPI _apiWrapper;

        public ScrutiniPage()
        {
            InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
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
            try
            {
                bool showDeletedDocuments = false;

                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        ScrutiniViewModel.IsLoadingScrutini = true;

                        if (CheckboxEliminati.IsChecked != null)
                            showDeletedDocuments = CheckboxEliminati.IsChecked.Value;
                    }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;


                ScrutiniDocumentsResult scrutiniDocumentsResult = await _apiWrapper.GetScrutiniDocuments(
                    cardResult.usrId.ToString());


                foreach (ScrutiniDocument document in scrutiniDocumentsResult.Documents)
                {
                    ScrutiniCheckResult scrutiniCheckResult = await _apiWrapper.CheckScrutinioDocument(
                        cardResult.usrId.ToString(),
                        document.hash);

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
                    CoreDispatcherPriority.Normal, () => { ScrutiniListView.ItemsSource = scrutiniDocumentsResult; }
                );
            }
            finally
            {
                {
                    {
                        await CoreApplication.MainView.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal, () => { ScrutiniViewModel.IsLoadingScrutini = false; }
                        );
                    }
                }
            }
        }

        public override async void AggiornaAction()
        {
            await Task.Run(async () => { await LoadData(); });
        }
    }
}