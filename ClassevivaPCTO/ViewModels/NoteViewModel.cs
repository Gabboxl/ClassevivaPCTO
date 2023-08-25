using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class NoteViewModel : ObservableObject
    {
        private bool _isLoadingNote = true;

        public bool IsLoadingNote
        {
            get { return _isLoadingNote; }
            set { SetProperty(ref _isLoadingNote, value); }
        }

        public NoteViewModel()
        {
        }
    }
}