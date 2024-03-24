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
            Highlights.Text = "ChangelogHighlighsSection".GetLocalizedStr(true);
            General.Text = "ChangelogGeneralSection".GetLocalizedStr(true);
            Dashboard.Text = "ChangelogDashboardSection".GetLocalizedStr(true);
            Agenda.Text = "ChangelogAgendaSection".GetLocalizedStr(true);
            Grades.Text = "ChangelogGradesSection".GetLocalizedStr(true);
            Absences.Text = "ChangelogAbsencesSection".GetLocalizedStr(true);
            NoticeBoard.Text = "ChangelogNoticeBoardSection".GetLocalizedStr(true);
        }
    }
}