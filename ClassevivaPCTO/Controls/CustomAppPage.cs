using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.ViewModels;

namespace ClassevivaPCTO.Controls
{
    public class CustomAppPage : Page
    {
        private static AppViewModel AppViewModel => AppViewModelHolder.GetViewModel();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

           // AppViewModel.UpdateUiAction = AggiornaAction;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //AppViewModel.UpdateUiAction = null;
        }

        public virtual void AggiornaAction()
        {
        }
    }
}
