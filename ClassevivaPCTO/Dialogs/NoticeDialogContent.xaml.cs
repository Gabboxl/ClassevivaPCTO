using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class NoticeDialogContent : Page
    {
        Notice CurrentNotice;

        private readonly IClassevivaAPI apiWrapper;

        public NoticeDialogContent(Notice notice)
        {
            this.InitializeComponent();

            CurrentNotice = notice;

            AttachmentsListView.ItemsSource = notice.attachments;

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as AppBarButton;
            var currentAttachment = senderbutton.DataContext as Attachment;


            byte[] bytes = await GetAttachmentAsBytes(currentAttachment);

            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(
                currentAttachment.fileName,
                Windows.Storage.CreationCollisionOption.ReplaceExisting
            );
            await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);
            await Windows.System.Launcher.LaunchFileAsync(file);
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e) {
        
        }


        private async Task<byte[]> GetAttachmentAsBytes(Attachment attachment)
        {
            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            var attachmentBinary = await apiWrapper.GetNoticeAttachment(
                cardResult.usrId.ToString(),
                CurrentNotice.pubId.ToString(),
                CurrentNotice.evtCode.ToString(),
                attachment.attachNum.ToString(),
                loginResult.token.ToString()
            );
            byte[] bytes = await attachmentBinary.Content.ReadAsByteArrayAsync();

            return bytes;
        }
    }
}
