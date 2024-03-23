using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Dialogs;
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
using ClassevivaPCTO.Helpers;
using CommunityToolkit.WinUI;
using Windows.Storage;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class NoticesListView : UserControl, INotifyPropertyChanged
    {
        private bool _showEmptyAlert = true;

        public bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            set { SetField(ref _showEmptyAlert, value); }
        }

        private readonly IClassevivaAPI apiWrapper;

        public EventHandler OnShouldUpdate
        {
            get { return (EventHandler) GetValue(OnShouldUpdateProperty); }
            set { SetValue(OnShouldUpdateProperty, value); }
        }

        private static readonly DependencyProperty OnShouldUpdateProperty =
            DependencyProperty.Register(nameof(OnShouldUpdate),
                typeof(EventHandler),
                typeof(NoticesListView),
                new PropertyMetadata(null, null));

        public List<Notice> ItemsSource
        {
            get { return (List<Notice>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(List<Notice>),
            typeof(NoticesListView),
            new PropertyMetadata(null, OnItemsSourceChanged)
        );

        private static void OnItemsSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            NoticesListView currentInstance = (NoticesListView) d;

            var newValue = e.NewValue as List<Notice>;

            var eventAdapters = newValue?.Select(evt => new NoticeAdapter(evt)).ToList();

            //save the scroll position
            var scrollViewer = currentInstance.listView.FindDescendant<ScrollViewer>();
            double horizontalOffset = scrollViewer.HorizontalOffset;
            double verticalOffset = scrollViewer.VerticalOffset;

            //update the listview contents
            currentInstance.listView.ItemsSource = eventAdapters;

            //restore the scroll position
            scrollViewer.ChangeView(horizontalOffset, verticalOffset, null);

            //set areSourcesEmpty
            currentInstance.ShowEmptyAlert = newValue == null || newValue.Count == 0;
        }

        public NoticesListView()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as Button;
            var currentNotice = (senderbutton.DataContext as NoticeAdapter).CurrentObject;

            //check whether the notice needs to be read, if yes create a flyout and with a text and button to confirm and display it on the button
            //if the user clicks the button, the flyout will be closed and the attachment will be read

            if (currentNotice.readStatus == false && !await ApplicationData.Current.LocalSettings.ReadAsync<bool>("SkipAskNoticeOpenEvent"))
            {
                //create a flyout
                var flyout = new Flyout();
                //create a textblock
                var textBlock = new TextBlock
                {
                    Text = "InfoNoticeFlyoutText".GetLocalizedStr(),
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

                var button = new Button
                {
                    Content = "GenericReadButton".GetLocalizedStr()
                };

                button.Click += async delegate
                {
                    flyout.Hide();
                    await Task.Run(() => ReadAndOpenNoticeDialog(currentNotice));
                };

                var closebutton = new Button
                {
                    Content = "GenericCloseButton".GetLocalizedStr()
                };

                closebutton.Click += delegate
                {
                    flyout.Hide();
                };

                var stackpanel = new StackPanel
                {
                    Children =
                    {
                        closebutton,
                        button,
                    }
                };

                stackpanel.Orientation = Orientation.Horizontal;
                stackpanel.Spacing = 10;

                flyout.Content = new StackPanel
                {
                    Children =
                    {
                        textBlock,
                        stackpanel,
                    }
                };

                //display the flyout on the clicked button
                flyout.ShowAt(senderbutton);
            }
            else
            {
                //apro la comunicazione in background
                await Task.Run(() => ReadAndOpenNoticeDialog(currentNotice));
            }
        }

        private async void ReadAndOpenNoticeDialog(Notice currentNotice)
        {
            Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

            //we need to read the notice first
            NoticeReadResult noticeReadResult =
                await apiWrapper.ReadNotice(cardResult.usrId.ToString(), currentNotice.pubId.ToString(),
                    currentNotice.evtCode);

            //execute on main UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var noticeDialogContent = new NoticeDialogContent(currentNotice, noticeReadResult);

                ContentDialog dialog = new()
                {
                    Title = currentNotice.cntTitle,
                    PrimaryButtonText = "GenericCloseButton".GetLocalizedStr(),
                    DefaultButton = ContentDialogButton.Primary,
                    RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme,
                    Content = noticeDialogContent,
                };

                try
                {
                    var result = await dialog.ShowAsync();

                    if (result == ContentDialogResult.Primary && !currentNotice.readStatus)
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