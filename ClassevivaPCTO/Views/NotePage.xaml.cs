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
using ClassevivaPCTO.Controls;

namespace ClassevivaPCTO.Views
{
    public sealed partial class NotePage : CustomAppPage
    {
        private NoteViewModel NoteViewModel { get; } = new();

        private readonly IClassevivaAPI _apiWrapper;

        public NotePage()
        {
            InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
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
                    CoreDispatcherPriority.Normal, () => { NoteViewModel.IsLoadingNote = true; }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;


                List<Note> notesResult = await _apiWrapper.GetAllNotes(
                    cardResult.usrId.ToString()
                );


                //update UI on UI thread
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () => { NotesListView.ItemsSource = notesResult; }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () => { NoteViewModel.IsLoadingNote = false; }
                );
            }
        }

        public override async void AggiornaAction()
        {
            await Task.Run(async () => { await LoadData(); });
        }
    }
}