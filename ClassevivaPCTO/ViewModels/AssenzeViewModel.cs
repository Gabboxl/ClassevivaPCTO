using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class AssenzeViewModel : ObservableObject
    {
        private bool _isLoadingAssenze = true;
        public bool IsLoadingAssenze
        {
            get { return _isLoadingAssenze; }
            set { SetProperty(ref _isLoadingAssenze, value); }
        }

        public AssenzeViewModel() { }
    }
}
