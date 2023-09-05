using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Adapters;


namespace ClassevivaPCTO.Views
{
    public sealed partial class Valutazioni : Page
    {
        private readonly IClassevivaAPI _apiWrapper;

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


            Card? cardResult = ViewModelHolder.GetViewModel().SingleCardResult;

            MainTitleTextBox.Text += VariousUtils.ToTitleCase(cardResult.firstName);

            await Task.Run(async () =>
            {
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

                /*
                var groupedData = from g in grades
                    join p in periods on g.periodPos equals p.periodPos
                    join s in subjects on g.subjectId equals s.id
                    group g by new {p.periodDesc, s.description}
                    into g
                    select new
                    {
                        Period = g.Key.periodDesc,
                        Subject = g.Key.description,
                        Grades = g.ToList()
                    };
                    */

                // Select unique Periods from Grade list
                var periodList = grades
                    .GroupBy(g => g.periodPos)
                    .Select(g => new
                    {
                        Period = periods.Single(p => p.periodPos == g.Key),
                        Subjects = g.GroupBy(s => s.subjectId)
                            .Select(sub => new
                            {
                                Subject = subjects.Single(s => s.id == sub.Key),
                                Grades = sub.Select(gr => gr).ToList()
                            }).ToList()
                    }).ToList();


                //run on UI thread
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    //add to ComboPeriodi every period of resultPeriods
                    foreach (var period in periodList)
                    {
                        SegmentedVoti.Items.Add(VariousUtils.UppercaseFirst(period.Period.periodDesc));


                    }

                    var periodGrades = periodList[0].Subjects;

                    //create a list of SubjectAdapter for every subject in periodGrades
                    var subjectAdapters = periodGrades.Select(subject =>
                        new SubjectAdapter(subject.Subject, subject.Grades)
                    ).ToList();

                    MainListView.ItemsSource = subjectAdapters;



                    ProgressRingVoti.Visibility = Visibility.Collapsed;
                });
            });
        }
    }
}