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
    public sealed partial class NoticesListView : UserControl
    {
        private readonly IClassevivaAPI apiWrapper;

        public EventHandler OnShouldUpdate
        {
            get { return (EventHandler)GetValue(OnShouldUpdateProperty); }
            set { SetValue(OnShouldUpdateProperty, value); }
        }

        private static readonly DependencyProperty OnShouldUpdateProperty =
            DependencyProperty.Register(nameof(OnShouldUpdate),
                typeof(EventHandler),
                typeof(NoticesListView),
                new PropertyMetadata(null, null));


        public List<Notice> ItemsSource
        {
            get { return (List<Notice>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(List<Notice>),
            typeof(NoticesListView),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged))
        );

        private static void OnItemsSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            NoticesListView currentInstance = (NoticesListView)d;

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
        }

        public NoticesListView()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as Button;
            var currentNotice = (senderbutton.DataContext as NoticeAdapter).CurrentObject;


            //check whether the notice needs to be read, if yes create a flyout and with a text and button to confirm and display it on the button
            //if the user clicks the button, the flyout will be closed and the attachment will be read

            if (currentNotice.readStatus == false)
            {
                //create a flyout
                var flyout = new Flyout();
                //create a textblock
                var textBlock = new TextBlock();
                textBlock.Text = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/InfoNoticeFlyoutText").ValueAsString;
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
                flyoutPresenterStyle.BasedOn = (Style)Application.Current.Resources["CautionFlyoutStyle"];


                flyout.FlyoutPresenterStyle = flyoutPresenterStyle;


                //create a button
                var button = new Button();
                button.Content = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/ReadAndOpenFlyoutText").ValueAsString;
                button.Click += async delegate
                {
                    //close the flyout
                    flyout.Hide();

                    //apro la comunicazione in background
                    await Task.Run(() => ReadAndOpenNoticeDialog(currentNotice));
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
                await Task.Run(() => ReadAndOpenNoticeDialog(currentNotice));
            }
        }


        private async void ReadAndOpenNoticeDialog(Notice currentNotice)
        {
            Card cardResult = ViewModelHolder.GetViewModel().SingleCardResult;


            //we need to read the notice first
            NoticeReadResult noticeReadResult =
                await apiWrapper.ReadNotice(cardResult.usrId.ToString(), currentNotice.pubId.ToString(),
                    currentNotice.evtCode);

            //execute on main UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var noticeDialogContent = new NoticeDialogContent(currentNotice, noticeReadResult);

                ContentDialog dialog = new ContentDialog();
                dialog.Title = currentNotice.cntTitle;
                dialog.PrimaryButtonText = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/CloseDialogButtonText").ValueAsString;
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
                dialog.Content = noticeDialogContent;
                    

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