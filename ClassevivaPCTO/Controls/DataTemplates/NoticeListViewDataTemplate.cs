using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls.DataTemplates
{
    internal partial class NoticeListViewDataTemplate : ResourceDictionary
    {
        public NoticeListViewDataTemplate()
        {
            InitializeComponent();
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
