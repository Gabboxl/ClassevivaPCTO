﻿using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ClassevivaPCTO.Helpers;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class NoticeDialogContent : Page
    {
        Notice CurrentNotice;
        NoticeReadResult CurrentReadResult;

        private readonly IClassevivaAPI _apiWrapper;

        public NoticeDialogContent(Notice notice, NoticeReadResult noticeReadResult)
        {
            InitializeComponent();

            CurrentNotice = notice;
            CurrentReadResult = noticeReadResult;

            if (CurrentNotice.attachments.Count == 0)
            {
                TitoloAttachments.Visibility = Visibility.Collapsed;
                AttachmentSeparator.Visibility = Visibility.Collapsed;
                NoAttachmentsPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                TitoloAttachments.Visibility = Visibility.Visible;
                AttachmentSeparator.Visibility = Visibility.Visible;
                NoAttachmentsPlaceholder.Visibility = Visibility.Collapsed;
            }

            AttachmentsListView.ItemsSource = notice.attachments;
            
            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();
            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
            
            MinWidth = 500;
            MaxWidth = 1000;
        }

        private async void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as AppBarButton;
            var currentAttachment = (NoticeAttachment) senderbutton.DataContext;

            await Task.Run(async () =>
            {
                byte[] bytes = await GetAttachmentAsBytes(currentAttachment);

                //run on ui thread
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(
                        currentAttachment.fileName,
                        Windows.Storage.CreationCollisionOption.ReplaceExisting
                    );
                    await Windows.Storage.FileIO.WriteBytesAsync(file, bytes);

                    var success = await Windows.System.Launcher.LaunchFileAsync(file);
                });
            });
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as AppBarButton;
            var currentAttachment = senderbutton.DataContext as NoticeAttachment;

            await Task.Run(async () =>
            {
                byte[] bytes = await GetAttachmentAsBytes(currentAttachment);

                //run on ui thread
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    savePicker.SuggestedStartLocation =
                        Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

                    savePicker.FileTypeChoices.Add("Allegato", new List<string>() {"."});
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
                });
            });
        }

        private async Task<byte[]> GetAttachmentAsBytes(NoticeAttachment attachment)
        {
            Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

            var attachmentBinary = await _apiWrapper.GetNoticeAttachment(
                cardResult.usrId.ToString(),
                CurrentNotice.pubId.ToString(),
                CurrentNotice.evtCode,
                attachment.attachNum.ToString()
            );

            byte[] bytes = await attachmentBinary.Content.ReadAsByteArrayAsync();

            return bytes;
        }
    }
}