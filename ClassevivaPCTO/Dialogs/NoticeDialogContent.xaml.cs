using ClassevivaPCTO.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class NoticeDialogContent : Page
    {
        Notice CurrentNotice;

        public NoticeDialogContent(Notice notice)
        {
            this.InitializeComponent();


            CurrentNotice = notice;

            AttachmentsListView.ItemsSource = notice.attachments;
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e) { }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e) { }
    }
}
