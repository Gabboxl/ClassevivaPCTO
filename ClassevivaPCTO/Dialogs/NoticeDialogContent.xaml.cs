using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ClassevivaPCTO.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class NoticeDialogContent : Page
    {
        private readonly Notice CurrentNotice;
        private readonly NoticeReadResult CurrentReadResult;

        private readonly IClassevivaAPI _apiWrapper;

        enum SignNoticeType
        {
            SIGN,
            SIGN_MESSAGE,
            SIGN_FILE,
        }

        //enum SignJoinNoticeStatus
        //{
        //    SUCCESS,
        //    REFUSED,
        //    REQUESTED
        //}

        private bool ShowSignAlert
        {
            get
            {
                return CurrentNotice.needSign ||
                       CurrentNotice.needFile ||
                       CurrentNotice.needReply;
            }
        }

        //private string JoinAlertMessage
        //{
        //    get
        //    {
        //        if (CurrentNotice.needJoin)
        //        {
        //            if (CurrentReadResult.reply.replJoin.GetValueOrDefault())
        //                return "NoticeDialogJoinSuccessMessage".GetLocalizedStr();

        //            return "NoticeDialogJoinRequestedMessage".GetLocalizedStr();
        //        }

        //        return "NoticeDialogJoinRequestedMessage".GetLocalizedStr();
        //    }
        //}

        //private string SignAlertMessage
        //{
        //    get
        //    {
        //        if (CurrentNotice.needJoin)
        //        {
        //            if (CurrentReadResult.reply.replJoin.GetValueOrDefault())
        //                return "NoticeDialogSignSuccessMessage".GetLocalizedStr();

        //            return "NoticeDialogSignRequestedMessage".GetLocalizedStr();
        //        }

        //        if (CurrentNotice.needSign)
        //        {
        //            if (CurrentReadResult.reply.replSign == null)
        //                return "NoticeDialogSignRequestedMessage".GetLocalizedStr();

        //            if (CurrentReadResult.reply.replSign.GetValueOrDefault())
        //                return "NoticeDialogSignSuccessMessage".GetLocalizedStr();

        //            return "NoticeDialogSignRefuseMessage".GetLocalizedStr();
        //        }

        //        return "NoticeDialogSignRequestedMessage".GetLocalizedStr();
        //    }
        //}

        private string InfobarMessage
        {
            get
            {
                if (CurrentNotice.needJoin && CurrentNotice.needSign)
                {
                    return "Per questa c sono richieste sia conferma che adesione";
                }

                if (CurrentNotice.needJoin && !CurrentNotice.needSign)
                {
                    return "NoticeDialogSignRequestedMessage".GetLocalizedStr();
                }

                if (CurrentNotice.needSign && !CurrentNotice.needJoin)
                {
                    return "NoticeDialogSignRequestedMessage".GetLocalizedStr();
                }

                return "Per questa c sono richieste sia conferma che adesione";
            }
        }

        private InfoBarSeverity JoinAlertSeverity
        {
            get
            {
                if (CurrentNotice.needJoin)
                    return CurrentReadResult.reply.replJoin.GetValueOrDefault() ? InfoBarSeverity.Success : InfoBarSeverity.Informational;

                return InfoBarSeverity.Informational;
            }
        }

        //private InfoBarSeverity SignAlertSeverityStatus
        //{
        //    get
        //    {

        //        if (CurrentNotice.needFile)
        //        {
        //            if (!string.IsNullOrEmpty(CurrentReadResult.reply.replFile))
        //                return InfoBarSeverity.Success;
        //        }

        //        if (CurrentNotice.needReply)
        //        {
        //            if (!string.IsNullOrEmpty(CurrentReadResult.reply.replText))
        //                return InfoBarSeverity.Success;
        //        }

        //        if (CurrentNotice.needSign)
        //        {
        //            if (CurrentReadResult.reply.replSign.GetValueOrDefault())
        //                return InfoBarSeverity.Success;
        //            return InfoBarSeverity.Informational;
        //        }

        //        return InfoBarSeverity.Informational;
        //    }
        //}

        private Visibility JoinButtonVisibility
        {
            get
            {
                if (!CurrentNotice.needJoin)
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }

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

            // confermata e firmata
            if (CurrentReadResult.reply.replSign.GetValueOrDefault())
            {
                ButtonSign.IsEnabled = false;
                ButtonSign.Label = "Confermato e firmato";
                ButtonRefuse.Visibility = Visibility.Collapsed;
            }

            //rifiutata
            if (!CurrentReadResult.reply.replSign.GetValueOrDefault())                 
            {
                ButtonRefuse.IsEnabled = false;
                ButtonRefuse.Label = "Rifiutato";
                ButtonJoin.Visibility = Visibility.Collapsed;
            }

            //aderito
            if (CurrentReadResult.reply.replJoin.GetValueOrDefault())
            {
                ButtonJoin.IsEnabled = false;
                ButtonJoin.Label = "Aderito";
            }

            AttachmentsListView.ItemsSource = notice.attachments;

            ButtonSign.Click += (sender, e) => ShowSignJoinRefuseFlyout(sender, e, false, false);
            ButtonJoin.Click += (sender, e) => ShowSignJoinRefuseFlyout(sender, e, true, false);
            ButtonRefuse.Click += (sender, e) => ShowSignJoinRefuseFlyout(sender, e, false, true);

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

        private void ShowSignJoinRefuseFlyout(object sender, RoutedEventArgs e, bool isJoin, bool isRefuse)
        {
            var flyout = new Flyout();

            var textBlock = new TextBlock
            {
                Text = GetActionText(isJoin, isRefuse),
                TextWrapping = TextWrapping.WrapWholeWords,
                Margin = new Thickness(0, 0, 0, 12)
            };

            var finalButton = new Button
            {
                Content = GetActionButtonContent(isJoin, isRefuse),
                MinWidth = 90,
                Style = (Style) Application.Current.Resources["AccentButtonStyle"],
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 12, 0),
            };

            finalButton.Click += async (sender, e) =>
            {
                var cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

                var noticeReadSignRequest = new NoticeReadSignRequest();

                if(isJoin)
                    noticeReadSignRequest.join = !isRefuse;
                else
                {
                    if (CurrentNotice.needFile)
                    {
                        noticeReadSignRequest.sign = !isRefuse;
                        noticeReadSignRequest.file = "file"; //TODO: implement file picker!!
                        noticeReadSignRequest.filename = "filename";
                    }else if (CurrentNotice.needSign)
                    {
                        noticeReadSignRequest.sign = !isRefuse;
                    }else if (CurrentNotice.needReply)
                    {
                        noticeReadSignRequest.sign = !isRefuse;
                        noticeReadSignRequest.text = "message"; //TODO: implement message edit!!
                    }
                }

                var result = await _apiWrapper.ReadNotice(cardResult.usrId.ToString(), CurrentNotice.pubId.ToString(),
                    CurrentNotice.evtCode, noticeReadSignRequest);

                CurrentReadResult.reply.replJoin = result.reply.replJoin;
                CurrentReadResult.reply.replSign = result.reply.replSign;
                CurrentReadResult.reply.replText = result.reply.replText;
                CurrentReadResult.reply.replFile = result.reply.replFile;

                flyout.Hide();
            };

            flyout.Content = new StackPanel
            {
                Children =
                {
                    textBlock,
                    finalButton
                }
            };

            //display the flyout on the clicked button
            flyout.ShowAt((FrameworkElement) sender);
        }

        private string GetActionText(bool isJoin, bool isRefuse)
        {
            if (isRefuse)
                return "NoticeDialogRefuseNoticeMessage".GetLocalizedStr();

            if(isJoin)
                return "NoticeDialogJoinNoticeMessage".GetLocalizedStr();

            if(CurrentNotice.needFile)
                return "NoticeDialogConfirmNoticeWithAttachmentMessage".GetLocalizedStr();
            if(CurrentNotice.needSign)
                return "NoticeDialogConfirmNoticeMessage".GetLocalizedStr();
            if(CurrentNotice.needReply)
                return "NoticeDialogConfirmNoticeWithMessage".GetLocalizedStr();

            return string.Empty;
            
        }

        private string GetActionButtonContent(bool isJoin, bool isRefuse)
        {
            if (isRefuse)
                return "NoticeDialogRefuseNotice".GetLocalizedStr();

            if(isJoin)
                return "NoticeDialogJoinNotice".GetLocalizedStr();

            if(CurrentNotice.needReply || CurrentNotice.needFile || CurrentNotice.needSign)
                return "NoticeDialogConfirmNotice".GetLocalizedStr(); 
            
            return string.Empty;
        }
    }
}