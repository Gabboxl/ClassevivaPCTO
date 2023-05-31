using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class NoticeDialogContent : Page
    {
        Notice CurrentNotice;
        NoticeReadResult CurrentReadResult;

        private readonly IClassevivaAPI apiWrapper;


        private string AllegatiText
        {
            get
            {
                if (CurrentNotice.attachments.Count == 0)
                {
                    return "Nessun allegato";
                }
                else
                {
                    return "Allegati";
                }
            }
        }

        public NoticeDialogContent(Notice notice, NoticeReadResult noticeReadResult)
        {
            this.InitializeComponent();

            CurrentNotice = notice;
            CurrentReadResult = noticeReadResult;

            AttachmentsListView.ItemsSource = notice.attachments;

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as AppBarButton;
            var currentAttachment = (NoticeAttachment)senderbutton.DataContext;

            byte[] bytes = await GetAttachmentAsBytes(currentAttachment);

            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(
                currentAttachment.fileName,
                Windows.Storage.CreationCollisionOption.ReplaceExisting
            );
            await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);
            await Windows.System.Launcher.LaunchFileAsync(file);
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as AppBarButton;
            var currentAttachment = senderbutton.DataContext as NoticeAttachment;


            byte[] bytes = await GetAttachmentAsBytes(currentAttachment);

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            savePicker.FileTypeChoices.Add("Allegato", new List<string>() { "." });
            savePicker.SuggestedFileName = currentAttachment.fileName;

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
        }


        private async Task<byte[]> GetAttachmentAsBytes(NoticeAttachment attachment)
        {
            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            var attachmentBinary = await apiWrapper.GetNoticeAttachment(
                cardResult.usrId.ToString(),
                CurrentNotice.pubId.ToString(),
                CurrentNotice.evtCode,
                attachment.attachNum.ToString(),
                loginResult.token
            );
            byte[] bytes = await attachmentBinary.Content.ReadAsByteArrayAsync();

            return bytes;
        }
    }
}