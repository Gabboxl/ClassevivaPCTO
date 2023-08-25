using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class DashboardPageViewModel : ObservableObject
    {
        private bool _isLoadingGrades = true;

        public bool IsLoadingGrades
        {
            get { return _isLoadingGrades; }
            set { SetProperty(ref _isLoadingGrades, value); }
        }

        private bool _isLoadingAgenda = true;

        public bool IsLoadingAgenda
        {
            get { return _isLoadingAgenda; }
            set { SetProperty(ref _isLoadingAgenda, value); }
        }

        private bool _isLoadingMedia = true;

        public bool IsLoadingMedia
        {
            get { return _isLoadingMedia; }
            set { SetProperty(ref _isLoadingMedia, value); }
        }

        private bool _isLoadingNotices = true;

        public bool IsLoadingNotices
        {
            get { return _isLoadingNotices; }
            set { SetProperty(ref _isLoadingNotices, value); }
        }

        public DashboardPageViewModel()
        {
        }
    }
}