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
using ClassevivaPCTO.Controls;
using ClassevivaPCTO.Helpers;
using Windows.Storage;
using CloneExtensions;
using CommunityToolkit.WinUI.Controls;

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

    public sealed partial class ValutazioniPage : CustomAppPage
    {
        private readonly IClassevivaAPI _apiWrapper;

        private List<PeriodList> _mergedPeriodList;

        private List<Grade> _sortedGrades;

        private ValutazioniViewModel ValutazioniViewModel { get; } = new();

        public ValutazioniPage()
        {
            InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient!);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SegmentedLayout.SelectedIndex = await ApplicationData.Current.LocalSettings.ReadAsync<int>("GradesLayoutMode");
            OrderByComboBox.SelectedIndex = await ApplicationData.Current.LocalSettings.ReadAsync<int>("GradesOrderMode");
            await Task.Run(async () => { await LoadData(); });

            SegmentedLayout.SelectionChanged += SegmentedLayout_OnSelectionChanged;
        }

        private async Task LoadData()
        {
            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () => { ValutazioniViewModel.IsLoadingValutazioni = true; }
                );

                var hideSubjectsWithoutGrades = await ApplicationData.Current.LocalSettings.ReadAsync<bool>("HideSubjectsWithoutGrades");

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

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
                            })
                            .Where(swg => !hideSubjectsWithoutGrades || swg.Grades.Count > 0)
                            .ToList()
                    }).ToList();

                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () => { UpdateUi(_sortedGrades); }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        ValutazioniViewModel.IsLoadingValutazioni = false;
                        ValutazioniViewModel.ShowShimmers = false;
                    }
                );
            }
        }

        private void UpdateUi()
        {
            var selectedPeriodIndex = SegmentedPeriodi.SelectedIndex;

            if (selectedPeriodIndex == -1) //page startup
            {
                foreach (var period in _mergedPeriodList)
                {
                    SegmentedPeriodi.Items?.Add(VariousUtils.UppercaseFirst(period.Period.periodDesc));
                }

                TitleFirstPerVal.Text = VariousUtils.UppercaseFirst(_mergedPeriodList[0].Period.periodDesc);
                if(_mergedPeriodList.Count > 1) //TODO: rendere adattivo per singolo periodo o più periodi
                    TitleSecondPerVal.Text = VariousUtils.UppercaseFirst(_mergedPeriodList[1].Period.periodDesc);
                SegmentedPeriodi.SelectedIndex = 0;

                return;
            }

            int selectedOrderIndex = OrderByComboBox.SelectedIndex;

            if (SegmentedLayout.SelectedIndex == 1) // layout grouped by subject
            {
                List<SubjectWithGrades> mergedPeriodsSubjectsWithGrades;

                if (selectedPeriodIndex == 0)
                {
                    mergedPeriodsSubjectsWithGrades = _mergedPeriodList
                        .SelectMany(p => p.Subjects)
                        .GroupBy(s => s.Subject.id)
                        .Select(g => new SubjectWithGrades
                        {
                            Subject = g.First().Subject,
                            Grades = OrderGrades(g.SelectMany(s => s.Grades).ToList())
                        })
                        .ToList();
                }
                else
                {
                    mergedPeriodsSubjectsWithGrades = _mergedPeriodList[selectedPeriodIndex - 1].Subjects
                        .Select(swg => new SubjectWithGrades
                        {
                            Subject = swg.Subject,
                            Grades = OrderGrades(swg.Grades)
                        })
                        .ToList();
                }

                var subjectAdapters = mergedPeriodsSubjectsWithGrades.Select(subject =>
                    new SubjectAdapter(subject.Subject, subject.Grades)
                ).ToList();

                MainListView.Visibility = Visibility.Visible;
                GradesOnlyListView.Visibility = Visibility.Collapsed;

                MainListView.ItemsSource = subjectAdapters;
            }
            else
            {
                MainListView.Visibility = Visibility.Collapsed;
                GradesOnlyListView.Visibility = Visibility.Visible;

                List<Grade> sortedGrades;
                if (selectedPeriodIndex != 0)
                {
                    sortedGrades = _sortedGrades
                        .Where(g => g.periodPos == _mergedPeriodList[selectedPeriodIndex - 1].Period.periodPos)
                        .ToList();
                }
                else
                {
                    sortedGrades = _sortedGrades.GetClone();
                }

                GradesOnlyListView.ItemsSource = OrderGrades(sortedGrades);
            }

            return;

            List<Grade> OrderGrades(List<Grade> grades)
            {
                return selectedOrderIndex switch
                {
                    0 => grades.OrderByDescending(g => g.evtDate).ToList(), // Newest to oldest
                    1 => grades.OrderBy(g => g.evtDate).ToList(), // Oldest to newest
                    2 => grades.OrderByDescending(g => g.decimalValue).ToList(), // Highest to lowest
                    3 => grades.OrderBy(g => g.decimalValue).ToList(), // Lowest to highest
                    _ => grades
                };
            }
        }

        private void UpdateUi(List<Grade> grades)
        {
            UpdateUi();

            //update statistics
            var firstPeriodGrades = _mergedPeriodList[0].Subjects.SelectMany(s => s.Grades).ToList();
            var secondPeriodGrades = _mergedPeriodList[1].Subjects.SelectMany(s => s.Grades).ToList();

            var allGradesCount = grades.Count;
            var firstPeriodGradesCount = firstPeriodGrades.Count;
            var secondPeriodGradesCount = secondPeriodGrades.Count;

            var allGradesAverage = VariousUtils.CalcolaMedia(grades);
            var firstPeriodGradesAverage = VariousUtils.CalcolaMedia(firstPeriodGrades.ToList());
            var secondPeriodGradesAverage = VariousUtils.CalcolaMedia(secondPeriodGrades.ToList());

            ValutazioniViewModel.AverageTot = allGradesAverage;
            ValutazioniViewModel.AverageFirstPeriodo = firstPeriodGradesAverage;
            ValutazioniViewModel.AverageSecondPeriodo = secondPeriodGradesAverage;

            ProgressMediaTot.Value = float.IsNaN(allGradesAverage) ? 0 : allGradesAverage * 10;
            ProgressMediaPrimoPeriodo.Value = float.IsNaN(firstPeriodGradesAverage) ? 0 : firstPeriodGradesAverage * 10;
            ProgressMediaSecondoPeriodo.Value =
                float.IsNaN(secondPeriodGradesAverage) ? 0 : secondPeriodGradesAverage * 10;

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


        private void SegmentedPeriodi_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUi();
        }

        public override async void AggiornaAction()
        {
            await Task.Run(async () => { await LoadData(); });
        }

        private async void SegmentedLayout_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!IsLoaded || sender is Segmented {IsLoaded: false})
                return;

            UpdateUi();
            await ApplicationData.Current.LocalSettings.SaveAsync("GradesLayoutMode", SegmentedLayout.SelectedIndex);
        }

        private async void OrderByComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_sortedGrades == null || _sortedGrades.Count == 0)
                return;

            UpdateUi();
            await ApplicationData.Current.LocalSettings.SaveAsync("GradesOrderMode", OrderByComboBox.SelectedIndex);
        }
    }
}