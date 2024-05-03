using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class ValutazioniViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoadingValutazioni = true;

        [ObservableProperty]
        private bool _showShimmers = true;

        [ObservableProperty]
        private float _averageTot;

        [ObservableProperty]
        private float _averageFirstPeriodo;

        [ObservableProperty]
        private float _averageSecondPeriodo;
    }
}