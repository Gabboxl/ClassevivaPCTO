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
            General.Text = "ChangelogGeneralSection".GetLocalizedStr();
            Dashboard.Text = "ChangelogDashboardSection".GetLocalizedStr();
            Agenda.Text = "ChangelogAgendaSection".GetLocalizedStr();
            Grades.Text = "ChangelogGradesSection".GetLocalizedStr();
            Absences.Text = "ChangelogAbsencesSection".GetLocalizedStr();
            NoticeBoard.Text = "ChangelogNoticeBoardSection".GetLocalizedStr();
        }
    }
}