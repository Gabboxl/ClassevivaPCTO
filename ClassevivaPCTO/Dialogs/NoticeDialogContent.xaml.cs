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
           
            var senderbutton = sender as AppBarButton;
            var currentAttachment = senderbutton.DataContext as Attachment;


            //check whether the attachments needs to be read, if yes create a flyout and with a text and button to confirm and display it on the button
            //if the user clicks the button, the flyout will be closed and the attachment will be read
            //check if the attachment is already read
            if (CurrentNotice.readStatus == false)
            {
                //create a flyout
                var flyout = new Flyout();
                //create a textblock
                var textBlock = new TextBlock();
                textBlock.Text = "Prima di salvare l'allegato è necessario contrassegnare come letta la comunicazione. Confermi?";
                //create a button
                var button = new Button();
                button.Content = "Conferma";
                button.Click += async delegate
                {
                    //close the flyout
                    flyout.Hide();


                    //leggi il comunicato con API e poi salva l'allegato
                    //await ReadAttachment(currentAttachment);
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
                //display the flyout on the button
                flyout.ShowAt(senderbutton);
            }
            else
            {
                //vai avanti e salva l'allegato
                //await ReadAttachment(currentAttachment);
            }
        



            byte[] bytes = await GetAttachmentAsBytes(currentAttachment);

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
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
