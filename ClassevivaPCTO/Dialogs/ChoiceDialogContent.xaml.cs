using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class ChoiceDialogContent : Page
    {
        public int chosenIndex { get; set; }

        public ChoiceDialogContent(List<LoginChoice> loginChoices)
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
