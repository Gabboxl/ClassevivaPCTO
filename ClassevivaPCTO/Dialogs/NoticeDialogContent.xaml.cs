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

        enum SignJoinNoticeType
        {
            JOIN,
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

        private bool ShowJoinAlert
        {
            get
            {
                return CurrentNotice.needJoin ||
                       CurrentNotice.needSign ||
                       CurrentNotice.needFile ||
                       CurrentNotice.needReply;
            }
        }

        private string JoinAlertMessage
        {
            get
            {
                if (CurrentNotice.needJoin)
                {
                    if (CurrentReadResult.reply.replJoin.GetValueOrDefault())
                        return "JoinSuccessMessage".GetLocalizedStr();

                    return "JoinRequestedMessage".GetLocalizedStr();
                }

                if (CurrentNotice.needSign)
                {
                    if (CurrentReadResult.reply.replSign == null)
                        return "SignRequestedMessage".GetLocalizedStr();

                    if (CurrentReadResult.reply.replSign.GetValueOrDefault())
                        return "SignSuccessMessage".GetLocalizedStr();
                    return "SignRefuseMessage".GetLocalizedStr();
                }

                return "JoinRequestedMessage".GetLocalizedStr();
            }
        }

        private InfoBarSeverity JoinAlertSeverityStatus //status
        {
            get
            {
                if (CurrentNotice.needJoin)
                {
                    if (CurrentReadResult.reply.replJoin.GetValueOrDefault())
                        return InfoBarSeverity.Success;
                } // a join can't be refused

                if (CurrentNotice.needFile)
                {
                    if (!string.IsNullOrEmpty(CurrentReadResult.reply.replFile))
                        return InfoBarSeverity.Success;
                }

                if (CurrentNotice.needReply)
                {
                    if (!string.IsNullOrEmpty(CurrentReadResult.reply.replText))
                        return InfoBarSeverity.Success;
                }

                if (CurrentNotice.needSign)
                {
                    if (CurrentReadResult.reply.replSign.GetValueOrDefault())
                        return InfoBarSeverity.Success;
                    return InfoBarSeverity.Error;
                }

                return InfoBarSeverity.Informational;
            }
        }

        private Visibility JoinButtonsVisibility
        {
            get
            {
                switch (JoinAlertSeverityStatus)
                {
                    case InfoBarSeverity.Success:
                        return Visibility.Collapsed;
                    case InfoBarSeverity.Informational:
                        return Visibility.Visible;
                    case InfoBarSeverity.Error:
                        return Visibility.Collapsed;
                    default:
                        return Visibility.Visible;
                }
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

            AttachmentsListView.ItemsSource = notice.attachments;

            ButtonSign.Click += (sender, e) => ShowSignJoinRefuseFlyout(sender, e, false);
            ButtonJoin.Click += (sender, e) => ShowSignJoinRefuseFlyout(sender, e, false);
            ButtonRefuse.Click += (sender, e) => ShowSignJoinRefuseFlyout(sender, e, true);

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

        private void ShowSignJoinRefuseFlyout(object sender, RoutedEventArgs e, bool isRefuse)
        {
            var flyout = new Flyout();

            var textBlock = new TextBlock
            {
                Text = GetActionText(isRefuse),
                TextWrapping = TextWrapping.WrapWholeWords,
                Margin = new Thickness(0, 0, 0, 12)
            };

            var finalButton = new Button
            {
                Content = GetActionButtonContent(isRefuse),
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

                switch (GetSignJoinType())
                {
                    case SignJoinNoticeType.SIGN_FILE:
                        noticeReadSignRequest.sign = !isRefuse;
                        noticeReadSignRequest.file = "file"; //TODO: implement file picker!!
                        noticeReadSignRequest.filename = "filename";
                        break;
                    case SignJoinNoticeType.SIGN:
                        noticeReadSignRequest.sign = !isRefuse;
                        break;
                    case SignJoinNoticeType.SIGN_MESSAGE:
                        noticeReadSignRequest.sign = !isRefuse;
                        noticeReadSignRequest.text = "message"; //TODO: implement message edit!!
                        break;
                    case SignJoinNoticeType.JOIN:
                        noticeReadSignRequest.join = !isRefuse;
                        break;
                }

                await _apiWrapper.ReadNotice(cardResult.usrId.ToString(), CurrentNotice.pubId.ToString(),
                    CurrentNotice.evtCode, noticeReadSignRequest);

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

        private string GetActionText(bool isRefuse)
        {
            if (isRefuse)
                return "Per rifiutare la comunicazione, premi il pulsante sottostante.";

            switch (GetSignJoinType())
            {
                case SignJoinNoticeType.SIGN_FILE:
                    return "Per firmare con file allegato, premi il pulsante sottostante.";
                case SignJoinNoticeType.SIGN:
                    return "Per firmare la comunicazione, premi il pulsante sottostante.";
                case SignJoinNoticeType.SIGN_MESSAGE:
                    return "Per firmare con messaggio, premi il pulsante sottostante.";
                case SignJoinNoticeType.JOIN:
                    return "Per unirti alla comunicazione, premi il pulsante sottostante.";
                default:
                    return string.Empty;
            }
        }

        private string GetActionButtonContent(bool isRefuse)
        {
            if (isRefuse)
                return "Rifiuta";

            switch (GetSignJoinType())
            {
                case SignJoinNoticeType.SIGN_FILE or SignJoinNoticeType.SIGN or SignJoinNoticeType.SIGN_MESSAGE:
                    return "Firma";
                case SignJoinNoticeType.JOIN:
                    return "Unisciti";
                default:
                    return string.Empty;
            }
        }

        private SignJoinNoticeType? GetSignJoinType()
        {
            if (CurrentNotice.needJoin)
                return SignJoinNoticeType.JOIN;

            if (CurrentNotice.needReply)
                return SignJoinNoticeType.SIGN_MESSAGE;

            if (CurrentNotice.needFile)
                return SignJoinNoticeType.SIGN_FILE;

            if (CurrentNotice.needSign)
                return SignJoinNoticeType.SIGN;

            return null;
        }
    }
}