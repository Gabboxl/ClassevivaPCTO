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

        private bool _mostraSoloComAttive = false;
        public bool MostraSoloComAttive
        {
            get { return _mostraSoloComAttive; }
            set { SetProperty(ref _mostraSoloComAttive, value); }
        }

        public BachecaViewModel() { }
    }
}
