using ClassevivaPCTO.DataModels;
using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class OverviewControlViewModel : ObservableObject
    {
        private OverviewDataModel _currentOverviewData;

        public OverviewDataModel CurrentOverviewData
        {
            get { return _currentOverviewData; }
            set { SetProperty(ref _currentOverviewData, value); }
        }

        private OverviewResult _filteredOverviewResult;

        public OverviewResult FilteredOverviewResult
        {
            get { return _filteredOverviewResult; }
            set { SetProperty(ref _filteredOverviewResult, value); }
        }

        private bool _showEmptyAlert = true;

        public bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            set { SetProperty(ref _showEmptyAlert, value); }
        }
    }
}