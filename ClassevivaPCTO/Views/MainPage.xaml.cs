using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;

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
            get { return VariousUtils.UppercaseFirst(AppViewModel.LoginResult.firstName) + " " + VariousUtils.UppercaseFirst(AppViewModel.LoginResult.lastName); }
        }

        public string Codice
        {
            get { return AppViewModel.LoginResult.ident; }
        }

        public string Scuola
        {
            get
            {
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


            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;

            PersonPictureDashboard.DisplayName = VariousUtils.UppercaseFirst(loginResult.firstName) + " " + VariousUtils.UppercaseFirst(loginResult.lastName);


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

                //delete localsettings data in case of multiple account chosen
                if(ChoiceSaverService.LoadChoiceIdentAsync().Result != null)
                {
                    ChoiceSaverService.RemoveSavedChoiceIdent();
                }
            }

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }

        }

    }



}
