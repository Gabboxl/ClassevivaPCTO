using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class ValutazioniViewModel : ObservableObject
    {
        private bool _isLoadingValutazioni = true;

        public bool IsLoadingValutazioni
        {
            get { return _isLoadingValutazioni; }
            set { SetProperty(ref _isLoadingValutazioni, value); }
        }

        public ValutazioniViewModel()
        {
        }
    }
}