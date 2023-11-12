using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Helpers;


namespace ClassevivaPCTO.Views
{
    public struct PeriodList
    {
        public Period Period { get; set; }
        public List<SubjectWithGrades> Subjects { get; set; }
    }

    public struct SubjectWithGrades
    {
        public Subject Subject { get; set; }
        public List<Grade> Grades { get; set; }
    }

    public sealed partial class ValutazioniPage : Page
    {
        private readonly IClassevivaAPI _apiWrapper;

        private List<PeriodList> _mergedPeriodList;

        private List<Grade> _sortedGrades;


        private ValutazioniViewModel ValutazioniViewModel { get; } = new();

        public ValutazioniPage()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient!);

            SegmentedLayout.SelectionChanged += SegmentedVoti_OnSelectionChanged;

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await Task.Run(async () => { await LoadData(); });
        }


        private async Task LoadData()
        {
            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { ValutazioniViewModel.IsLoadingValutazioni = true; }
                );

                Card? cardResult = ViewModelHolder.GetViewModel().SingleCardResult;


                Grades2Result grades2Result = await _apiWrapper.GetGrades(
                    cardResult.usrId.ToString()
                );

                PeriodsResult resultPeriods = await _apiWrapper.GetPeriods(
                    cardResult.usrId.ToString()
                );

                SubjectsResult resultSubjects = await _apiWrapper.GetSubjects(
                    cardResult.usrId.ToString()
                );

                var gradesRaw = grades2Result.Grades;

                //order grades by date descending
                _sortedGrades = gradesRaw
                    .OrderByDescending(g => g.evtDate)
                    .ToList();

                var subjects = resultSubjects.Subjects;


                //find all periods with same periodDesc value
                var samePeriods = resultPeriods.Periods
                    .GroupBy(p => p.periodDesc)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .OrderBy(p => p.periodPos)
                    .ToList();

                samePeriods.ForEach(p => p.periodDesc = (samePeriods.IndexOf(p) + 1) + "° " + p.periodDesc);


                // Select unique Periods from Grade list
                _mergedPeriodList = resultPeriods.Periods
                    .GroupBy(p => p.periodPos)
                    .Select(p => new PeriodList
                    {
                        Period = p.First(),
                        Subjects = subjects
                            .GroupBy(s => s.id)
                            .Select(sub => new SubjectWithGrades
                            {
                                Subject = sub.First(),
                                Grades = _sortedGrades
                                    .Where(g => g.subjectId == sub.Key && g.periodPos == p.Key)
                                    .ToList()
                            }).ToList()
                    }).ToList();


                //update UI on UI thread
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { UpdateUi(_sortedGrades, subjects); }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        ValutazioniViewModel.IsLoadingValutazioni = false;
                        ValutazioniViewModel.ShowShimmers = false;
                    }
                );
            }
        }

        private void UpdateUi()
        {
            var periodIndex = SegmentedVoti.SelectedIndex;

            if (periodIndex == -1)
            {
                foreach (var period in _mergedPeriodList)
                {
                    SegmentedVoti.Items.Add(VariousUtils.UppercaseFirst(period.Period.periodDesc));
                }

                TitleFirstPerVal.Text = VariousUtils.UppercaseFirst(_mergedPeriodList[0].Period.periodDesc);
                TitleSecondPerVal.Text = VariousUtils.UppercaseFirst(_mergedPeriodList[1].Period.periodDesc);

                SegmentedVoti.SelectedIndex = 0;
                SegmentedVoti.IsEnabled = true;

            }
            else if (periodIndex == 0)
            {
                SegmentedLayout.IsEnabled = true;

                if (SegmentedLayout.SelectedIndex == 1)
                {
                    MainListView.Visibility = Visibility.Visible;
                    GradesOnlyListView.Visibility = Visibility.Collapsed;

                    //create a list of SubjectAdapter from periodGrades by merging periods together and count subjects as one distinct subject
                    var mergetdPeriodsSubjects = _mergedPeriodList
                        .SelectMany(p => p.Subjects)
                        .GroupBy(s => s.Subject.id)
                        .Select(g => new SubjectWithGrades
                        {
                            Subject = g.First().Subject,
                            Grades = g.SelectMany(s => s.Grades).ToList()
                        })
                        .ToList();

                    var subjectAdapters = mergetdPeriodsSubjects.Select(subject =>
                        new SubjectAdapter(subject.Subject, subject.Grades)
                    ).ToList();

                    MainListView.ItemsSource = subjectAdapters;
                }
                else
                {
                    MainListView.Visibility = Visibility.Collapsed;
                    GradesOnlyListView.Visibility = Visibility.Visible;
                    GradesOnlyListView.ItemsSource = _sortedGrades;
                }
            }
            else
            {
                SegmentedLayout.IsEnabled = false;
                MainListView.Visibility = Visibility.Visible;
                GradesOnlyListView.Visibility = Visibility.Collapsed;


                var periodGrades = _mergedPeriodList[periodIndex - 1].Subjects;

                //create a list of SubjectAdapter for every subject in periodGrades
                var subjectAdapters = periodGrades.Select(subject =>
                    new SubjectAdapter(subject.Subject, subject.Grades)
                ).ToList();

                MainListView.ItemsSource = subjectAdapters;
            }
        }

        private void UpdateUi(List<Grade> grades, List<Subject> subjects)
        {
            //update main UI
            UpdateUi();


            //update statistics

            var firstPeriodGrades = _mergedPeriodList[0].Subjects.SelectMany(s => s.Grades).ToList();
            var secondPeriodGrades = _mergedPeriodList[1].Subjects.SelectMany(s => s.Grades).ToList();

            //count all grades
            var allGradesCount = grades.Count;
            var firstPeriodGradesCount = firstPeriodGrades.Count;
            var secondPeriodGradesCount = secondPeriodGrades.Count;

            //average of all grades
            var allGradesAverage = VariousUtils.CalcolaMedia(grades);

            var firstPeriodGradesAverage = VariousUtils.CalcolaMedia(
                firstPeriodGrades.ToList()
            );

            var secondPeriodGradesAverage = VariousUtils.CalcolaMedia(
                secondPeriodGrades.ToList());

            //update viewmodel
            ValutazioniViewModel.AverageTot = allGradesAverage;
            ValutazioniViewModel.AverageFirstPeriodo = firstPeriodGradesAverage;
            ValutazioniViewModel.AverageSecondPeriodo = secondPeriodGradesAverage;

            //set progressrings value
            ProgressMediaTot.Value = float.IsNaN(allGradesAverage) ? 0 : allGradesAverage * 10;
            ProgressMediaPrimoPeriodo.Value = float.IsNaN(firstPeriodGradesAverage) ? 0 : firstPeriodGradesAverage * 10;
            ProgressMediaSecondoPeriodo.Value =
                float.IsNaN(secondPeriodGradesAverage) ? 0 : secondPeriodGradesAverage * 10;

            //set grades count
            string valutazioniPlAllgrad =
                allGradesCount == 1 ? "GradeSingular".GetLocalizedStr() : "GradesPlural".GetLocalizedStr();
            string valutazioniPlurale1 = firstPeriodGradesCount == 1
                ? "GradeSingular".GetLocalizedStr()
                : "GradesPlural".GetLocalizedStr();
            string valutazioniPlurale2 = secondPeriodGradesCount == 1
                ? "GradeSingular".GetLocalizedStr()
                : "GradesPlural".GetLocalizedStr();

            NumTotVal.Text = string.Format("{0} " + valutazioniPlAllgrad, allGradesCount.ToString());
            NumFirstPerVal.Text = string.Format("{0} " + valutazioniPlurale1, firstPeriodGradesCount.ToString());
            NumSecondPerVal.Text = string.Format("{0} " + valutazioniPlurale2, secondPeriodGradesCount.ToString());
        }


        private void SegmentedVoti_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUi();
        }

        private async void ReloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () => { await LoadData(); });
        }
    }
}