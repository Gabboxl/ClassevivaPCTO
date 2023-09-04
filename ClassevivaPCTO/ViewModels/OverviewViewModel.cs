using ClassevivaPCTO.DataModels;
using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class OverviewViewModel : ObservableObject
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

        private bool _areSourcesEmpty = true;

        public bool AreSourcesEmpty
        {
            get { return _areSourcesEmpty; }
            set { SetProperty(ref _areSourcesEmpty, value); }
        }

        public OverviewViewModel()
        {
        }
    }
}