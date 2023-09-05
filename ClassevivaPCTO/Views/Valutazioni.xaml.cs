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


namespace ClassevivaPCTO.Views
{
    public class PeriodList
    {
        public Period Period { get; set; }
        public List<SubjectWithGrades> Subjects { get; set; }
    }

    public class SubjectWithGrades
    {
        public Subject Subject { get; set; }
        public List<Grade> Grades { get; set; }
    }

    public sealed partial class Valutazioni : Page
    {
        private readonly IClassevivaAPI _apiWrapper;

        private List<PeriodList> _periodList;

        private ValutazioniViewModel ValutazioniViewModel { get; } = new();

        public Valutazioni()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient!);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await Task.Run(async () => { await LoadData(); });


            Card? cardResult = ViewModelHolder.GetViewModel().SingleCardResult;

            MainTitleTextBox.Text += VariousUtils.ToTitleCase(cardResult.firstName);

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

                var grades = grades2Result.Grades;
                var periods = resultPeriods.Periods;
                var subjects = resultSubjects.Subjects;


                // Select unique Periods from Grade list
                _periodList = grades
                    .GroupBy(g => g.periodPos)
                    .Select(g => new PeriodList
                    {
                        Period = periods.Single(p => p.periodPos == g.Key),
                        Subjects = g.GroupBy(s => s.subjectId)
                            .Select(sub => new SubjectWithGrades
                            {
                                Subject = subjects.Single(s => s.id == sub.Key),
                                Grades = sub.Select(gr => gr).ToList()
                            }).ToList()
                    }).ToList();


                //update UI on UI thread
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { UpdateUi(); }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => { ValutazioniViewModel.IsLoadingValutazioni = false; }
                );
            }
        }

        private void UpdateUi()
        {
            var periodIndex = SegmentedVoti.SelectedIndex;

            if (periodIndex == -1)
            {
                foreach (var period in _periodList)
                {
                    SegmentedVoti.Items.Add(VariousUtils.UppercaseFirst(period.Period.periodDesc));
                }

                SegmentedVoti.SelectedIndex = 0;

                SegmentedVoti.IsEnabled = true;
            } else if (periodIndex == 0)
            {
                //create a list of SubjectAdapter from periodGrades by merging periods together and count subjects as one distinct subject
                var mergetdPeriodsSubjects = _periodList
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
                var periodGrades = _periodList[periodIndex - 1].Subjects;

                //create a list of SubjectAdapter for every subject in periodGrades
                var subjectAdapters = periodGrades.Select(subject =>
                    new SubjectAdapter(subject.Subject, subject.Grades)
                ).ToList();

                MainListView.ItemsSource = subjectAdapters;
            }
        }


        private void SegmentedVoti_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUi();
        }

        private async void ReloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }
    }
}