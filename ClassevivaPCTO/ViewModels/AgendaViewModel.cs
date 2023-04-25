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

        private bool _areSourcesEmpty = true;
        public bool AreSourcesEmpty
        {
            get { return _areSourcesEmpty; }
            set { SetProperty(ref _areSourcesEmpty, value); }
        }


        public AgendaViewModel() { }
    }
}
