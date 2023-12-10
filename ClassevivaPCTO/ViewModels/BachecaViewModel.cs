using System.Collections.Generic;
using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class BachecaViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<Notice> _noticesToShow;

        [ObservableProperty]
        private List<string> _categories;

        [ObservableProperty]
        private bool _isLoadingBacheca = true;

        [ObservableProperty]
        private bool _mostraComInattive;
    }
}