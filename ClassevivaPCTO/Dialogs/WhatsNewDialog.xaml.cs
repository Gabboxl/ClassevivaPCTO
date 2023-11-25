using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ClassevivaPCTO.Helpers;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public WhatsNewDialog()
        {
            // TODO: Update the contents of this dialog every time you release a new version of the app
            RequestedTheme = ((FrameworkElement) Window.Current.Content).RequestedTheme;
            InitializeComponent();

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