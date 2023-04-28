using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ClassevivaPCTO.Controls
{
    public sealed partial class NoticesListView : UserControl
    {

        public List<NoticeAdapter> ItemsSource
        {
            get { return (List<NoticeAdapter>)GetValue(ItemsSourceProperty); }
            set {
                listView.ItemsSource = value;
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(List<NoticeAdapter>),
                typeof(NoticesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));


        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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



            //create and show a custom dialog
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "side";
            dialog.PrimaryButtonText = "OK";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content =
                notice.cntTitle;

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
