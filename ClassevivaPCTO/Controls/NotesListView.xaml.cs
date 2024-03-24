using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using ClassevivaPCTO.Helpers;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;

namespace ClassevivaPCTO.Controls
{
    public enum DisplayMode
    {
        Compact,
        Full,
        Default
    }

    //public enum HeaderType
    //{
    //    None,
    //    Single,
    //    Sticky
    //}

    public sealed partial class NotesListView : UserControl, INotifyPropertyChanged
    {
        private readonly IClassevivaAPI _apiWrapper;

        public EventHandler OnShouldUpdate
        {
            get { return (EventHandler) GetValue(OnShouldUpdateProperty); }
            set { SetValue(OnShouldUpdateProperty, value); }
        }

        private static readonly DependencyProperty OnShouldUpdateProperty =
            DependencyProperty.Register(nameof(OnShouldUpdate),
                typeof(EventHandler),
                typeof(NotesListView),
                new PropertyMetadata(null, null));

        public DisplayMode Mode
        {
            get { return (DisplayMode) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        private static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(DisplayMode), typeof(NotesListView),
                new PropertyMetadata(DisplayMode.Default, OnModeChanged));

        public bool EnableEmptyAlert
        {
            get { return (bool) GetValue(EnableEmptyAlertProperty); }
            set { SetValue(EnableEmptyAlertProperty, value); }
        }

        private static readonly DependencyProperty EnableEmptyAlertProperty =
            DependencyProperty.Register(
                nameof(EnableEmptyAlert),
                typeof(bool),
                typeof(NotesListView),
                new PropertyMetadata(false, null));

        public bool EnableStickyHeader
        {
            get { return (bool) GetValue(EnableStickyHeaderProperty); }
            set { SetValue(EnableStickyHeaderProperty, value); }
        }

        private static readonly DependencyProperty EnableStickyHeaderProperty =
            DependencyProperty.Register(nameof(EnableStickyHeader), typeof(bool), typeof(NotesListView),
                new PropertyMetadata(false, null));


        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotesListView currentInstance = (NotesListView) d;
            DisplayMode newMode = (DisplayMode) e.NewValue;

            // Handle the mode change, e.g., update the UI
            currentInstance.UpdateMode(newMode);
        }

        private void UpdateMode(DisplayMode mode)
        {
            // Update your UserControl UI based on the new mode
        }

        private bool _showEmptyAlert;

        public bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            private set { SetField(ref _showEmptyAlert, value); }
        }

        public List<Note> ItemsSource
        {
            get { return (List<Note>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(List<Note>),
            typeof(NotesListView),
            new PropertyMetadata(null, OnItemsSourceChanged)
        );

        private CollectionViewSource GroupedItems { get; set; }

        private static async Task<ObservableCollection<GroupInfoList>> GetNotesGroupedAsync(
            IEnumerable<NoteAdapter> noteAdapters)
        {
            var query = from item in noteAdapters

                // Group the items returned from the query, sort and select the ones you want to keep
                group item by item.CurrentObject.evtCode
                into g
                orderby g.Key descending

                //TODO: forse ordinare ulteriormente ogni gruppo per data?

                //prendo il long name dell'enum con attributo ApiValueAttribute
                select new GroupInfoList(g) {Key = g.Key.ToString().GetLocalizedStr("plur")};

            return new ObservableCollection<GroupInfoList>(query);
        }

        private static async void OnItemsSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            NotesListView currentInstance = (NotesListView) d;

            var newValue = e.NewValue as List<Note>;

            var noteAdapters = newValue?.Select(evt => new NoteAdapter(evt)).ToList();

            //save the scroll position
            var scrollViewer = currentInstance.listView.FindDescendant<ScrollViewer>();
            double horizontalOffset = scrollViewer.HorizontalOffset;
            double verticalOffset = scrollViewer.VerticalOffset;

            //object perchè GroupedItems.Source può essere un IEnumerable oppure un IList
            object finalNotesObject = currentInstance.EnableStickyHeader
                ? await GetNotesGroupedAsync(noteAdapters)
                : noteAdapters;

            currentInstance.GroupedItems = new CollectionViewSource
            {
                IsSourceGrouped = currentInstance.EnableStickyHeader, //TODO: settare proprietà da dependencyproperty
                Source = finalNotesObject //in base al valore di IsSourceGrouped, Source può essere un IEnumerable oppure un IList
            };

            //update the listview contents
            currentInstance.listView.ItemsSource = currentInstance.GroupedItems.View;
            
            //reset the selection
            currentInstance.listView.SelectedIndex = -1;

            //restore the scroll position
            scrollViewer.ChangeView(horizontalOffset, verticalOffset, null);

            //update the empty state
            currentInstance.ShowEmptyAlert =
                (newValue == null || newValue.Count == 0) && currentInstance.EnableEmptyAlert;
        }

        public NotesListView()
        {
            InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = (Button) sender;
            var currentNote = ((NoteAdapter) senderbutton.DataContext).CurrentObject;

            //check whether the notice needs to be read, if yes create a flyout and with a text and button to confirm and display it on the button
            //if the user clicks the button, the flyout will be closed and the attachment will be read

            if (currentNote.readStatus == false)
            {
                //create a flyout
                var flyout = new Flyout();

                var textBlock = new TextBlock
                {
                    Text = "InfoNoteFlyoutText".GetLocalizedStr(),
                    TextWrapping = TextWrapping.WrapWholeWords,
                    Margin = new Thickness(0, 0, 0, 12)
                };

                //create a flyoutpresenterstyle with the SystemFillColorCautionBackgroundBrush color and set it to the flyout
                var flyoutPresenterStyle = new Style(typeof(FlyoutPresenter));

                //we are not using the theme from code anymore because it doesn't change when the theme is changed on Windows 10, so we are using a XAML resource style hook insteal (see se CautionFlyoutStyle in App.xaml)
                //    flyoutPresenterStyle.Setters.Add(new Setter(FlyoutPresenter.BackgroundProperty, (Windows.UI.Xaml.Media.Brush)Application.Current.Resources["SystemFillColorCautionBackgroundBrush"]));

                //make the flyout wrap the text vertically
                flyoutPresenterStyle.Setters.Add(new Setter(ScrollViewer.HorizontalScrollModeProperty,
                    ScrollMode.Disabled));
                flyoutPresenterStyle.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty,
                    ScrollBarVisibility.Disabled));

                //make the flyoutPresenterStyle based on the default one
                flyoutPresenterStyle.BasedOn = (Style) Application.Current.Resources["CautionFlyoutStyle"];
                flyout.FlyoutPresenterStyle = flyoutPresenterStyle;

                //create a button
                var button = new Button
                {
                    Content = "ReadAndOpenFlyoutText".GetLocalizedStr()
                };

                button.Click += async delegate
                {
                    //chiudo il flyout e apro la comunicazione in background
                    flyout.Hide();

                    await Task.Run(() => ReadAndOpenNoteDialog(currentNote));
                };

                //add the textblock and the button to the flyout
                flyout.Content = new StackPanel
                {
                    Children =
                    {
                        textBlock,
                        button
                    }
                };

                //display the flyout on the clicked button
                flyout.ShowAt(senderbutton);
            }
            else
            {
                //apro la comunicazione in background
                await Task.Run(() => ReadAndOpenNoteDialog(currentNote));
            }
        }

        private async void ReadAndOpenNoteDialog(Note currentNote)
        {
            Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

            //we need to read the notice first
            ReadNoteResult readNoteResult =
                await _apiWrapper.ReadNote(cardResult.usrId.ToString(), currentNote.evtCode.ToString(),
                    currentNote.evtId.ToString());

            //execute on main UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var noteDialogContent = new NoteDialogContent(currentNote, readNoteResult);

                MetadataControl metadataControlTitle = new()
                {
                    Separator = " • ",
                    AccessibleSeparator = ",",
                    Items = new[]
                    {
                        new MetadataItem { Label = currentNote.authorName },
                        new MetadataItem { Label = currentNote.evtCode.ToString().GetLocalizedStr("sing") + "GenericEventOnDayPreposition".GetLocalizedStr() + currentNote.evtDate.ToString("dd/MM/yyyy"), },
                    }
                };

                ContentDialog dialog = new()
                {
                    Title = metadataControlTitle,
                    PrimaryButtonText = "CloseDialogButtonText".GetLocalizedStr(),
                    DefaultButton = ContentDialogButton.Primary,
                    RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme,
                    Content = noteDialogContent,
                    Width = 1200
                };

                try
                {
                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        //raise OnShouldUpdate event
                        OnShouldUpdate?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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