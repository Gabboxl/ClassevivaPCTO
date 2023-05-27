using System;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Deserializers;
using Newtonsoft.Json;
using System.Collections.Generic;
using Refit;

namespace ClassevivaPCTO.Views
{
    public sealed partial class Note : Page
    {
        private NoteViewModel NoteViewModel { get; } = new NoteViewModel();

        private readonly IClassevivaAPI apiWrapper;

        public Note()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NoteViewModel.IsLoadingNote = true;

            
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
                    NoteViewModel.IsLoadingNote = true;

                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;


            ApiResponse<string> notesResult = await apiWrapper.GetAllNotes(
                cardResult.usrId.ToString(),
                loginResult.token
            );


            List<Utils.Note> notesList = JsonConvert.DeserializeObject<List<Utils.Note>>(notesResult.Content, new NoteDeserializer());


            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    NotesListView.ItemsSource = notesList;

                    NoteViewModel.IsLoadingNote = false;
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
