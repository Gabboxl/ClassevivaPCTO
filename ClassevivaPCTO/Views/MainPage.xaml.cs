using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ClassevivaPCTO.Views
{
    public sealed partial class MainPage : Page
    {
        private AppViewModel _appViewModel;

        public NavigationViewViewModel NavigationViewViewModel { get; } = new();

        public string FirstName
        {
            get
            {
                return VariousUtils.ToTitleCase(_appViewModel.LoginResult.firstName)
                       + " "
                       + VariousUtils.ToTitleCase(_appViewModel.LoginResult.lastName);
            }
        }

        public string Codice
        {
            get { return _appViewModel.LoginResult.ident; }
        }

        public string Scuola
        {
            get
            {
                Card? card = _appViewModel.SingleCardResult;

                return card.schName + " " + card.schDedication + " [" + card.schCode + "]";
            }
        }

        public MainPage()
        {
            InitializeComponent();

            this.DataContext = this; //DataContext = ViewModel;
            Initialize();

            _appViewModel = AppViewModelHolder.GetViewModel();
        }

        private void Initialize()
        {
            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            AppTitleTextBlock.Text = "" + AppInfo.Current.DisplayInfo.DisplayName;
            Window.Current.SetTitleBar(AppTitleBar);

            //remove the solid-colored backgrounds behind the caption controls and system back button
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            NavigationViewViewModel.Initialize(contentFrame, navigationView, KeyboardAccelerators);

            LoginResultComplete? loginResult = AppViewModelHolder.GetViewModel().LoginResult;

            /* PersonPictureDashboard.DisplayName =
                 VariousUtils.ToTitleCase(loginResult.firstName)
                 + " "
                 + VariousUtils.ToTitleCase(loginResult.lastName); */

            //pagina di default
            NavigationService.Navigate(typeof(DashboardPage));
            
            // intercetta il tasto F5 per il refresh della pagina corrente
            this.KeyDown += MainPage_KeyDown;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        //{
        //    VariousUtils.DoLogout();
        //}

        private void MainPage_KeyDown(object sender, KeyRoutedEventArgs args)
        {
            switch (args.Key)
            {
                case VirtualKey.F5:

                    NavigationViewViewModel.RefreshCurrentPageData();

                    break;
            }
        }

        private void MainRefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationViewViewModel.RefreshCurrentPageData();
        }
    }
}