﻿using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Dialogs;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ClassevivaPCTO.Controls
{
    public sealed partial class NoticesListView : UserControl
    {
        private readonly IClassevivaAPI apiWrapper;

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

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        //appbarbutton onclick handler
        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            var senderbutton = sender as AppBarButton;
            var currentNotice = (senderbutton.DataContext as NoticeAdapter).CurrentObject;



            //check whether the notice needs to be read, if yes create a flyout and with a text and button to confirm and display it on the button
            //if the user clicks the button, the flyout will be closed and the attachment will be read

            if (currentNotice.readStatus == false)
            {
                //create a flyout
                var flyout = new Flyout();
                //create a textblock
                var textBlock = new TextBlock();
                textBlock.Text = "La comunicazione verrà contrassegnata come letta sul server. Continuare?";
                textBlock.TextWrapping = TextWrapping.WrapWholeWords;
                textBlock.Margin = new Thickness(0, 0, 0, 12);
                
                

                //create a flyoutpresenterstyle with the SystemFillColorCautionBackgroundBrush color and set it to the flyout
                var flyoutPresenterStyle = new Style(typeof(FlyoutPresenter));
                flyoutPresenterStyle.Setters.Add(new Setter(FlyoutPresenter.BackgroundProperty, (Windows.UI.Xaml.Media.Brush)Application.Current.Resources["SystemFillColorCautionBackgroundBrush"]));

                //make the flyout wrap the text vertically
                flyoutPresenterStyle.Setters.Add(new Setter(ScrollViewer.HorizontalScrollModeProperty, ScrollMode.Disabled));
                flyoutPresenterStyle.Setters.Add(new Setter(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled));


                //make the flyoutPresenterStyle based on the default one
                flyoutPresenterStyle.BasedOn = (Style)Application.Current.Resources["DefaultFlyoutPresenterStyle"];


                flyout.FlyoutPresenterStyle = flyoutPresenterStyle;


                //create a button
                var button = new Button();
                button.Content = "Leggi e apri";
                button.Click += async delegate
                {
                    //close the flyout
                    flyout.Hide();

                    //apro la comunicazione
                    ReadAndOpenNoticeDialog(currentNotice);
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
                ReadAndOpenNoticeDialog(currentNotice);
            }


        }


        public async void ReadAndOpenNoticeDialog(Notice currentNotice)
        {
            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];


            //we need to read the notice first
            NoticeReadResult noticeReadResult =
             await apiWrapper.ReadNotice(cardResult.usrId.ToString(), currentNotice.pubId.ToString(),
             currentNotice.evtCode,
                loginResult.token.ToString());


            var noticeDialogContent = new NoticeDialogContent(currentNotice, noticeReadResult);

            ContentDialog dialog = new ContentDialog();
            dialog.Title = currentNotice.cntTitle;
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
