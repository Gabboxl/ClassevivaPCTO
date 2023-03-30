using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class AppViewModel : ObservableObject
    {
        private LoginResult loginResult;
        public LoginResult LoginResult
        {
            get => loginResult;
            set => SetProperty(ref loginResult, value);
        }


        private CardsResult cardsResult;
        public CardsResult CardsResult
        {
            get => cardsResult;
            set => SetProperty(ref cardsResult, value);
        }

    }
}
