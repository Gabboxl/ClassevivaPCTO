using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class ScrutiniViewModel : ObservableObject
    {
        private bool _isLoadingScrutini = true;

        public bool IsLoadingScrutini
        {
            get { return _isLoadingScrutini; }
            set { SetProperty(ref _isLoadingScrutini, value); }
        }
    }
}