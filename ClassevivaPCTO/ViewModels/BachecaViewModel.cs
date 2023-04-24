using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class BachecaViewModel : ObservableObject
    {
        private bool _isLoadingBacheca = true;
        public bool IsLoadingBacheca
        {
            get { return _isLoadingBacheca; }
            set { SetProperty(ref _isLoadingBacheca, value); }
        }

        public BachecaViewModel() { }
    }
}
