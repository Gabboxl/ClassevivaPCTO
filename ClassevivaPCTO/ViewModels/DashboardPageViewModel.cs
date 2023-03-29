using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


using ClassevivaPCTO.Helpers;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

using WinUI = Microsoft.UI.Xaml.Controls;

namespace ClassevivaPCTO.ViewModels
{
    public class DashboardPageViewModel : ObservableObject
    {

        private bool _isLoadingGrades = false;
        public bool IsLoadingGrades
        {
            get { return _isLoadingGrades; }
            set { SetProperty(ref _isLoadingGrades, value); }
        }

        private bool _isLoadingAgenda = false;
        public bool IsLoadingAgenda
        {
            get { return _isLoadingAgenda; }
            set { SetProperty(ref _isLoadingAgenda, value); }
        }

        private bool _isLoadingMedia = false;
        public bool IsLoadingMedia
        {
            get { return _isLoadingMedia; }
            set { SetProperty(ref _isLoadingMedia, value); }
        }

        public DashboardPageViewModel()
        {
        }

        public void Initialize(Frame frame, WinUI.NavigationView navigationView, IList<KeyboardAccelerator> keyboardAccelerators)
        {

        }


 
    }
}
