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

        private bool _mostraComInattive = false;
        public bool MostraComInattive
        {
            get { return _mostraComInattive; }
            set { SetProperty(ref _mostraComInattive, value); }
        }

        public BachecaViewModel() { }
    }
}
