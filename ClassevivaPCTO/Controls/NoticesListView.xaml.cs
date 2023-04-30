using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ClassevivaPCTO.Controls
{
    public sealed partial class NoticesListView : UserControl
    {
        public List<Notice> ItemsSource
        {
            get { return (List<Notice>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(List<Notice>),
            typeof(NoticesListView),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged))
        );

        private static void OnItemsSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            NoticesListView currentInstance = d as NoticesListView;

            var newValue = e.NewValue as List<Notice>;

            var eventAdapters = newValue?.Select(evt => new NoticeAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;
        }

        public NoticesListView()
        {
            this.InitializeComponent();
        }

        //appbarbutton onclick handler
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as NoticeAdapter;
            var notice = item.CurrentObject;

            var noticeDialogContent = new NoticeDialogContent(notice);

            ContentDialog dialog = new ContentDialog();
            dialog.Title = notice.cntTitle;
            dialog.PrimaryButtonText = "Chiudi";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            dialog.Content = noticeDialogContent;

            //dialog.FullSizeDesired = true;
            dialog.Width = 1200;

            try
            {
                var result = await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }
    }
}
