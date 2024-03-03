using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class ScrutiniViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoadingScrutini = true;
    }
}