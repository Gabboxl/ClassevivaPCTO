using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ClassevivaPCTO.Helpers;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public WhatsNewDialog()
        {
            RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme;
            InitializeComponent();

            Title = "SettingsCardWhatsNewHeader".GetLocalizedStr();
            PrimaryButtonText = "GenericCloseButton".GetLocalizedStr();
            Highlights.Text = "ChangelogHighlighsSection".GetLocalizedStr();
        }
    }
}