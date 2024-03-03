using ClassevivaPCTO.DataModels;
using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class OverviewControlViewModel : ObservableObject
    {
        [ObservableProperty]
        private OverviewDataModel _currentOverviewData;

        [ObservableProperty]
        private OverviewResult _filteredOverviewResult;

        [ObservableProperty]
        private bool _showEmptyAlert = true;
    }
}