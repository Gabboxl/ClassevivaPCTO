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


        private float _averageTot;

        public float AverageTot
        {
            get { return _averageTot; }
            set { SetProperty(ref _averageTot, value); }
        }


        private float _averageFirstPeriodo;

        public float AverageFirstPeriodo
        {
            get { return _averageFirstPeriodo; }
            set { SetProperty(ref _averageFirstPeriodo, value); }
        }


        private float _averageSecondPeriodo;

        public float AverageSecondPeriodo
        {
            get { return _averageSecondPeriodo; }
            set { SetProperty(ref _averageSecondPeriodo, value); }
        }

        public ValutazioniViewModel()
        {
        }
    }
}