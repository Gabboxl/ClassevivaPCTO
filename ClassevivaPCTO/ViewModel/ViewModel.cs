using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModel
{
    public class ViewModel : ObservableObject
    {
        private LoginResult loginResult;


        public LoginResult LoginResult
        {
            get => loginResult;
            set => SetProperty(ref loginResult, value);
        }

    }
}
