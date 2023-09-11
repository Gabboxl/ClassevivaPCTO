using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;
using ScrutinioAdapter = ClassevivaPCTO.Adapters.ScrutinioAdapter;


namespace ClassevivaPCTO.Controls
{
    public sealed partial class ScrutiniListView : UserControl, INotifyPropertyChanged
    {
        public bool EnableEmptyAlert
        {
            get { return (bool) GetValue(EnableEmptyAlertProperty); }
            set
            {
                SetValue(EnableEmptyAlertProperty, value);
                _showEmptyAlert = value;
            }
        }

        private static readonly DependencyProperty EnableEmptyAlertProperty =
            DependencyProperty.Register(
                nameof(EnableEmptyAlert),
                typeof(bool),
                typeof(LessonsListView),
                new PropertyMetadata(false, null));

        private static bool _showEmptyAlert;

        public bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            set
            {
                SetField(ref _showEmptyAlert, value);
            }
        }


        private readonly IClassevivaAPI apiWrapper;


        public ScrutiniDocumentsResult ItemsSource
        {
            get { return (ScrutiniDocumentsResult) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(ScrutiniDocumentsResult),
            typeof(ScrutiniListView),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged))
        );

        private static void OnItemsSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            ScrutiniListView currentInstance = (ScrutiniListView) d;

            var newValue = e.NewValue as ScrutiniDocumentsResult;

            var scrutiniAdapters = newValue.Documents?.Select(evt => new ScrutinioAdapter(evt)).ToList();
            var schoolReportsAdapters = newValue.SchoolReports?.Select(evt => new SchoolReportAdapter(evt)).ToList();

            //save the scroll position
            var scrollViewer = currentInstance.listViewScrutini.FindDescendant<ScrollViewer>();
            double horizontalOffset = scrollViewer.HorizontalOffset;
            double verticalOffset = scrollViewer.VerticalOffset;


            //update the listview contents
            currentInstance.listViewScrutini.ItemsSource = scrutiniAdapters;
            currentInstance.listViewSchoolReports.ItemsSource = schoolReportsAdapters;


            //restore the scroll position
            scrollViewer.ChangeView(horizontalOffset, verticalOffset, null);

            //update the empty state
            currentInstance.ShowEmptyAlert = (newValue == null || (newValue.Documents.Count == 0 && newValue.SchoolReports.Count == 0)) && _showEmptyAlert;
        }

        public ScrutiniListView()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as Button;
            var currentScrutinio = (senderbutton.DataContext as ScrutinioAdapter).CurrentObject;

            await Task.Run(async () =>
            {
                var getFileResult = await GetScrutinioFileAsBytes(currentScrutinio);

                byte[] bytes = getFileResult.Item1;

                //run on ui thread
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(
                        getFileResult.Item2,
                        Windows.Storage.CreationCollisionOption.ReplaceExisting
                    );
                    await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);
                    await Windows.System.Launcher.LaunchFileAsync(file);
                });
            });
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as Button;
            var currentScrutinio = (senderbutton.DataContext as ScrutinioAdapter).CurrentObject;

            await Task.Run(async () =>
            {
                var getFileResult = await GetScrutinioFileAsBytes(currentScrutinio);

                byte[] bytes = getFileResult.Item1;

                //run on ui thread
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    savePicker.SuggestedStartLocation =
                        Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

                    savePicker.FileTypeChoices.Add("Allegato", new List<string>() {"."});
                    savePicker.SuggestedFileName = getFileResult.Item2;

                    Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                    if (file != null)
                    {
                        // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                        Windows.Storage.CachedFileManager.DeferUpdates(file);

                        //scrivo il file
                        await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);

                        Windows.Storage.Provider.FileUpdateStatus status =
                            await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                        if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                        {
                            //completato
                        }
                        else
                        {
                            //trakkar errore?
                        }
                    }
                    else
                    {
                        //we need to track the error
                    }
                });
            });
        }


        private async Task<(byte[], string)> GetScrutinioFileAsBytes(ScrutiniDocument document)
        {
            Card? cardResult = ViewModelHolder.GetViewModel().SingleCardResult;

            var attachmentBinary = await apiWrapper.GetScrutinioDocumentFile(
                cardResult.usrId.ToString(),
                document.hash
            );

            //we get the filename from the content disposition header, as we don't know the extension of the file
            var filename = attachmentBinary.Content.Headers.ContentDisposition.FileName;

            //remove the quotes from the filename
            filename = filename.Substring(1, filename.Length - 2);


            byte[] bytes = await attachmentBinary.Content.ReadAsByteArrayAsync();

            return (bytes, filename);
        }

        private void ButtonOpenReport_OnClick(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as Button;
            var currentSchoolReport = (senderbutton.DataContext as SchoolReportAdapter).CurrentObject;

            var link = currentSchoolReport.viewLink;

            //open link in browser
            Windows.System.Launcher.LaunchUriAsync(new Uri(link));
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}