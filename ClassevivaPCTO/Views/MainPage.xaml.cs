using System;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml.Input;

using ClassevivaPCTO.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ClassevivaPCTO.Services;

using WinUI = Microsoft.UI.Xaml.Controls;
using System.Linq;
using ClassevivaPCTO.Views;
using System.Threading;
using ClassevivaPCTO.ViewModels;

namespace ClassevivaPCTO.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public AppViewModel AppViewModel { get; set; }

        public NavigationViewViewModel NavigationViewViewModel { get; } = new NavigationViewViewModel();


        public string FirstName
        {
            get { return VariousUtils.UppercaseFirst(AppViewModel.LoginResult.FirstName) + " " + VariousUtils.UppercaseFirst(AppViewModel.LoginResult.LastName); }
        }

        public string Codice
        {
            get { return AppViewModel.LoginResult.Ident; }
        }

        public string Scuola
        {
            get {
                Card card = AppViewModel.CardsResult.Cards[0];

                return card.schName + " " + card.schDedication + " [" + card.schCode + "]"; 
            
            }
        }



        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = this; //DataContext = ViewModel;
            Initialize();

            

            this.AppViewModel = ViewModelHolder.getViewModel();
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


            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;

            PersonPictureDashboard.DisplayName = VariousUtils.UppercaseFirst(loginResult.FirstName) + " " + VariousUtils.UppercaseFirst(loginResult.LastName);

           
            //pagina di default
            NavigationService.Navigate(typeof(Views.DashboardPage));

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

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));




        private async void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {

            var loginCredential = new CredUtils().GetCredentialFromLocker();

            if (loginCredential != null)
            {
                loginCredential.RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota


                var vault = new Windows.Security.Credentials.PasswordVault();

                vault.Remove(new Windows.Security.Credentials.PasswordCredential(
                    "classevivapcto", loginCredential.UserName.ToString(), loginCredential.Password.ToString()));

            }

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }

        }

        private void navigationView_ItemInvoked(WinUI.NavigationView sender, WinUI.NavigationViewItemInvokedEventArgs args)
        {

        }
    }



}
