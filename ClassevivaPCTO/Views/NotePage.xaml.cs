using System;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace ClassevivaPCTO.Views
{
    public sealed partial class NotePage : Page
    {
        private NoteViewModel NoteViewModel { get; } = new();

        private readonly IClassevivaAPI apiWrapper;

        public NotePage()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await Task.Run(async () => { await LoadData(); });
        }

        private async Task LoadData()
        {
            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { NoteViewModel.IsLoadingNote = true; }
                );

                Card? cardResult = ViewModelHolder.GetViewModel().SingleCardResult;


                List<Utils.Note> notesResult = await apiWrapper.GetAllNotes(
                    cardResult.usrId.ToString()
                );


                //update UI on UI thread
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { NotesListView.ItemsSource = notesResult; }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { NoteViewModel.IsLoadingNote = false; }
                );
            }
        }

        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(async () => { await LoadData(); });
        }
    }
}