using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModel;
using Refit;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassevivaPCTO
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class DettaglioVoti : Page
    {
        public DettaglioVoti()
        {
            this.InitializeComponent();


            //titolo title bar
            AppTitleTextBlock.Text = "Dettaglio voti - " + AppInfo.Current.DisplayInfo.DisplayName;
            Window.Current.SetTitleBar(AppTitleBar);

            var currentView = SystemNavigationManager.GetForCurrentView();
            //currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;


            currentView.BackRequested += (s, e) =>
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            };


        }



        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;


            var api = RestService.For<IClassevivaAPI>("https://web.spaggiari.eu/rest/v1");

            string fixedId = new CvUtils().GetCode(loginResult.Ident);

            var resultGrades = await api.GetGrades(fixedId, loginResult.Token.ToString());

            var resultPeriods = await api.GetPeriods(fixedId, loginResult.Token.ToString());


            MainTextBox.Text = "Dettaglio voti di " + VariousUtils.UppercaseFirst(loginResult.FirstName);



            var gradesGroupedByMaterie = resultGrades.Grades.GroupBy(x => x.subjectDesc).Select(grp => grp.ToList()).ToList();


            //rimuovo tutto al pivot di test
            PivotPeriodi.Items.Clear();


            foreach (Period period in resultPeriods.Periods)
            {

                PivotItem pvt = new PivotItem();
                pvt.Name = period.periodDesc;

                pvt.Header = VariousUtils.UppercaseFirst(period.periodDesc);

                var stack = new StackPanel();
                stack.Orientation = Orientation.Vertical;

                var textbrock = new TextBlock();
                textbrock.Text = "Periodo dal " + period.dateStart.ToString("dd/MM/yyy") + " al " + period.dateEnd.ToString("dd/MM/yyy");
                textbrock.Margin = new Thickness(8);

                //add items to the stackpanel
                stack.Children.Add(textbrock);

                Pivot innerPivot = new Pivot();

                foreach(List<Grade> materiaWithGrades in gradesGroupedByMaterie) {
                    PivotItem innerpvtItem = new PivotItem();
                    innerpvtItem.Header = VariousUtils.UppercaseFirst(materiaWithGrades[0].subjectDesc);

                    foreach(Grade grade in materiaWithGrades)
                    {
                        ListView lw = new ListView();
                        //lw.tem
                    }
                    //TODO: aggiungere la listview con i voti al pivotitem

                    innerPivot.Items.Add(innerpvtItem);
                    
                }
                stack.Children.Add(innerPivot);


                pvt.Content = stack;

                PivotPeriodi.Items.Add(pvt);


            }


            ProgressRingVoti.Visibility = Visibility.Collapsed;
            PivotPeriodi.Visibility = Visibility.Visible;   



        }

        public void GoBack(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }


    }
}
