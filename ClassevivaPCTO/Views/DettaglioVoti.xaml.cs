using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Refit;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassevivaPCTO.Views
{
    public sealed partial class DettaglioVoti : Page
    {
        Grades2Result _grades2Result;

        public DettaglioVoti()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            var api = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);

            _grades2Result = await api.GetGrades(
                cardResult.usrId.ToString(),
                loginResult.token
            );

            var resultPeriods = await api.GetPeriods(
                cardResult.usrId.ToString(),
                loginResult.token
            );

            MainTextBox.Text =
                "Dettaglio voti di " + VariousUtils.ToTitleCase(loginResult.firstName);

            //add to ComboPeriodi every period of resultPeriods
            foreach (Period period in resultPeriods.Periods)
            {
                ComboPeriodi.Items.Add(VariousUtils.UppercaseFirst(period.periodDesc));
            }

            //seleziono il primo elemento iniziale - questo chiamerà il metodo ComboPeriodi_SelectionChanged subito
            ComboPeriodi.SelectedItem = ComboPeriodi.Items[0];

            ProgressRingVoti.Visibility = Visibility.Collapsed;
        }

        private void ComboPeriodi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AggiornaComboMaterie();
        }

        private void ComboMaterie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AggiornaListViewVoti();
        }

        private void AggiornaComboMaterie()
        {
            //pulisco il ComboMaterie
            ComboMaterie.Items.Clear();

            var gradesGroupedByPeriodoDesc = _grades2Result.Grades
                .OrderBy(x => x.evtDate)
                .GroupBy(x => x.periodDesc)
                .Select(grp => grp.ToList())
                .ToList();

            int c = 0;

            foreach (var periodWithGrades in gradesGroupedByPeriodoDesc)
            {
                //if (periodWithGrades[0].periodDesc.Equals(ComboPeriodi.SelectedValue))

                if (c == ComboPeriodi.SelectedIndex)
                {
                    var gradesGroupedByMaterie = periodWithGrades
                        .OrderByDescending(x => x.evtDate)
                        .GroupBy(x => x.subjectDesc)
                        .Select(grp => grp.ToList()) //creo una lista di liste delle materie con i voti
                        .ToList();

                    foreach (List<Grade> materiaWithGrades in gradesGroupedByMaterie)
                    {
                        ComboMaterie.Items.Add(
                            VariousUtils.UppercaseFirst(materiaWithGrades[0].subjectDesc)
                        );
                    }

                    ComboMaterie.SelectedItem = ComboMaterie.Items[0];

                    return;
                }

                c++;
            }
        }

        private void AggiornaListViewVoti()
        {
            var gradesGroupedByPeriodoDesc = _grades2Result.Grades
                .OrderBy(x => x.evtDate)
                .GroupBy(x => x.periodDesc)
                .Select(grp => grp.ToList()) //metto ogni gruppo di periodo in una lista a parte
                .ToList();

            int c = 0;

            foreach (var periodWithGrades in gradesGroupedByPeriodoDesc)
            {
                //if (periodWithGrades[0].periodDesc.Equals(ComboPeriodi.SelectedValue))

                if (c == ComboPeriodi.SelectedIndex)
                {
                    var gradesGroupedByMaterie = periodWithGrades
                        .OrderByDescending(x => x.evtDate)
                        .GroupBy(x => x.subjectDesc)
                        .Select(grp => grp.ToList())
                        .ToList();

                    int y = 0;

                    foreach (List<Grade> materiaWithGrades in gradesGroupedByMaterie)
                    {
                        if (y == ComboMaterie.SelectedIndex)
                        {
                            ListViewVoti.ItemsSource = materiaWithGrades;
                        }

                        y++;
                    }

                    return;
                }

                c++;
            }
        }

        public void GoBack(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = (Frame)Window.Current.Content;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }
    }
}
