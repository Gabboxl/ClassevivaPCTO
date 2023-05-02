using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class OverviewViewModel : ObservableObject
    {
        private bool _areSourcesEmpty = true;
        public bool AreSourcesEmpty
        {
            get { return _areSourcesEmpty; }
            set { SetProperty(ref _areSourcesEmpty, value); }
        }

        public OverviewViewModel() { }
    }
}
