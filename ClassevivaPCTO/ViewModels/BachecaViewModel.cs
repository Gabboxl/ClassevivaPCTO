using System.Collections.Generic;
using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public class BachecaViewModel : ObservableObject
    {
        private List<Notice> _noticesToShow;

        public List<Notice> NoticesToShow
        {
            get { return _noticesToShow; }
            set { SetProperty(ref _noticesToShow, value); }
        }

        private List<string> _categories;

        public List<string> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        private bool _isLoadingBacheca = true;

        public bool IsLoadingBacheca
        {
            get { return _isLoadingBacheca; }
            set { SetProperty(ref _isLoadingBacheca, value); }
        }

        private bool _mostraComInattive = false;

        public bool MostraComInattive
        {
            get { return _mostraComInattive; }
            set { SetProperty(ref _mostraComInattive, value); }
        }
    }
}