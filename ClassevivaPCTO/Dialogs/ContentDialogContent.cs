using ClassevivaPCTO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class ContentDialogContent : Page
    {
        public int chosenIndex { get; set; }

        public ContentDialogContent(List<LoginChoice> loginChoices)
        {
            this.InitializeComponent();


            //make a list of string from each Name propety of loginChoices
            var choices = loginChoices.Select(x => x.name).ToList();
            ComboChoice.ItemsSource = choices;
            
            //select the first one
            ComboChoice.SelectedIndex = 0;

        }
    }
}
