using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class DashboardPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoadingGrades = true;

        [ObservableProperty]
        private bool _isLoadingAgenda = true;

        [ObservableProperty]
        private bool _isLoadingMedia = true;

        [ObservableProperty]
        private bool _isLoadingNotices = true;
    }
}