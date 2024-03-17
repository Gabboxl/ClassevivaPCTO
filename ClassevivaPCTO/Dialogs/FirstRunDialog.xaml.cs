using ClassevivaPCTO.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        public FirstRunDialog()
        {
            RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme;
            InitializeComponent();

            Title = "FirstRunDialogTitle".GetLocalizedStr();
            PrimaryButtonText = "FirstRunDialogPrimaryButtonText".GetLocalizedStr();
        }
    }
}