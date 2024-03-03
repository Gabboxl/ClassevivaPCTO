using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class NoteViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoadingNote = true;
    }
}