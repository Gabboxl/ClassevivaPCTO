using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;


namespace ClassevivaPCTO.Controls
{
    public enum DisplayMode
    {
        Compact,
        Full,
        Default
    }


    public sealed partial class NotesListView : UserControl
    {
        private readonly IClassevivaAPI apiWrapper;

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
            get { return (DisplayMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(DisplayMode), typeof(NotesListView), new PropertyMetadata(DisplayMode.Default, OnModeChanged));


        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NotesListView currentInstance = (NotesListView)d;
            DisplayMode newMode = (DisplayMode)e.NewValue;

            // Handle the mode change, e.g., update the UI
            currentInstance.UpdateMode(newMode);
        }

        private void UpdateMode(DisplayMode mode)
        {
            // Update your UserControl UI based on the new mode
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
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged))
        );

        private static void OnItemsSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            NotesListView currentInstance = (NotesListView) d;

            var newValue = e.NewValue as List<Note>;

            var eventAdapters = newValue?.Select(evt => new NoteAdapter(evt)).ToList();

            //save the scroll position
            var scrollViewer = currentInstance.listView.FindDescendant<ScrollViewer>();
            double horizontalOffset = scrollViewer.HorizontalOffset;
            double verticalOffset = scrollViewer.VerticalOffset;


            //update the listview contents
            currentInstance.listView.ItemsSource = eventAdapters;


            //restore the scroll position
            scrollViewer.ChangeView(horizontalOffset, verticalOffset, null);

        }

        public NotesListView()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        //appbarbutton onclick handler
        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as AppBarButton;
            var currentNote = (senderbutton.DataContext as NoteAdapter).CurrentObject;


            //check whether the notice needs to be read, if yes create a flyout and with a text and button to confirm and display it on the button
            //if the user clicks the button, the flyout will be closed and the attachment will be read

            if (currentNote.readStatus == false)
            {
                //create a flyout
                var flyout = new Flyout();
                //create a textblock
                var textBlock = new TextBlock();
                textBlock.Text = "La nota verrà contrassegnata come letta sul server. Continuare?";
                textBlock.TextWrapping = TextWrapping.WrapWholeWords;
                textBlock.Margin = new Thickness(0, 0, 0, 12);


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
                var button = new Button();
                button.Content = "Leggi e apri";
                button.Click += async delegate
                {
                    //close the flyout
                    flyout.Hide();

                    //apro la comunicazione in background
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
            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;


            //we need to read the notice first
            ReadNoteResult readNoteResult =
                await apiWrapper.ReadNote(cardResult.usrId.ToString(), currentNote.evtCode.ToString(),
                    currentNote.evtId.ToString(),
                    loginResult.token);

            //execute on main UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var noteDialogContent = new NoteDialogContent(currentNote, readNoteResult);

                ContentDialog dialog = new ContentDialog();
                dialog.Title = currentNote.evtCode.GetLongName();
                dialog.PrimaryButtonText = "Chiudi";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme;
                dialog.Content = noteDialogContent;

                //dialog.FullSizeDesired = true;
                dialog.Width = 1200;

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
    }
}