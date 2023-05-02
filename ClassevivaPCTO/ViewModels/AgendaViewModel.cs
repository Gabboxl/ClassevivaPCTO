using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class AgendaViewModel : ObservableObject
    {
        private bool _isLoadingAgenda = true;
        public bool IsLoadingAgenda
        {
            get { return _isLoadingAgenda; }
            set { SetProperty(ref _isLoadingAgenda, value); }
        }


        public AgendaViewModel() { }
    }
}
