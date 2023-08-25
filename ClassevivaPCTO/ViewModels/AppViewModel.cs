using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class AppViewModel : ObservableObject
    {
        private LoginResultComplete _loginResult;

        public LoginResultComplete LoginResult
        {
            get => _loginResult;
            set => SetProperty(ref _loginResult, value);
        }

        private Card _singleCardsResult;

        public Card SingleCardResult
        {
            get => _singleCardsResult;
            set => SetProperty(ref _singleCardsResult, value);
        }

        private CardsResult _cardsResult;

        public CardsResult CardsResult
        {
            get => _cardsResult;
            set => SetProperty(ref _cardsResult, value);
        }
    }
}