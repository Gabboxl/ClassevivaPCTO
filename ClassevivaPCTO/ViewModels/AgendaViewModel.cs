using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class AgendaViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoadingAgenda = true;
    }
}