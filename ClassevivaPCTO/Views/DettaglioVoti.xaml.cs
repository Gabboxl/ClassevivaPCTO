using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ClassevivaPCTO.Views
{
    public sealed partial class DettaglioVoti : Page
    {
        private Grades2Result _grades2Result;

        private readonly IClassevivaAPI apiWrapper;

        public DettaglioVoti()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            Card cardResult = ViewModelHolder.GetViewModel().SingleCardResult;

            MainTitleTextBox.Text += VariousUtils.ToTitleCase(cardResult.firstName);


            Task.Run(async () =>
            {
                _grades2Result = await apiWrapper.GetGrades(
                    cardResult.usrId.ToString()
                );

                var resultPeriods = await apiWrapper.GetPeriods(
                    cardResult.usrId.ToString()
                );


                //run on UI thread
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    //add to ComboPeriodi every period of resultPeriods
                    foreach (Period period in resultPeriods.Periods)
                    {
                        ComboPeriodi.Items.Add(VariousUtils.UppercaseFirst(period.periodDesc));
                    }

                    //seleziono il primo elemento iniziale - questo chiamerà il metodo ComboPeriodi_SelectionChanged subito
                    ComboPeriodi.SelectedItem = ComboPeriodi.Items[0];

                    ProgressRingVoti.Visibility = Visibility.Collapsed;
                });
            });
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