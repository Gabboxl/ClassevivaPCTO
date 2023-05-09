using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class OverviewListView : UserControl
    {
        public OverviewViewModel OverviewViewModel { get; } = new OverviewViewModel();

        public OverviewDataModel ItemsSource
        {
            get { return (OverviewDataModel)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(OverviewDataModel),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (OverviewListView) d;

            var newValue = (OverviewDataModel) e.NewValue;

            currentInstance.OverviewViewModel.CurrentOverviewData = newValue;

            //OverviewResult overviewResult = newValue.OverviewData;
            OverviewResult overviewResult = currentInstance.OverviewViewModel.CurrentOverviewData.OverviewData;

            OverviewResult filteredOverviewResults = overviewResult;

            if (newValue.FilterDate != null)
            {
                //filteredOverviewResults = new OverviewResult();
                filteredOverviewResults.AbsenceEvents = overviewResult.AbsenceEvents.Where(abs => abs.evtDate.Date == newValue.FilterDate.Date).ToList();

                //filter agenda events if the selected date is between the start and end date of the event
                filteredOverviewResults.AgendaEvents = overviewResult.AgendaEvents
                    .Where(
                        agendaEvent =>
                            agendaEvent.evtDatetimeBegin.Date <= newValue.FilterDate.Date && agendaEvent.evtDatetimeEnd.Date >= newValue.FilterDate.Date //be sure to use the Date property instead of the DateTime one because the time is not important and there is risk some oneday-only events are not shown
                    )
                    .ToList();

                filteredOverviewResults.Grades = overviewResult.Grades.Where(grade => grade.evtDate == newValue.FilterDate.Date).ToList();
                
                //questo per le lezioni in teoria non serve
                //filteredOverviewResults.Lessons = overviewResult.Lessons.Where(les => les.evtDate == newValue.FilterDate.Date).ToList();
            }



            //set the data to the listviews
            
            currentInstance.AbsencesListView.ItemsSource = filteredOverviewResults.AbsenceEvents;
            currentInstance.GradesListView.ItemsSource = filteredOverviewResults.Grades;
            currentInstance.LessonsListView.ItemsSource = filteredOverviewResults.Lessons;
            currentInstance.AgendaListView.ItemsSource = filteredOverviewResults.AgendaEvents;
            //listview note da mettere


            currentInstance.OverviewViewModel.FilteredOverviewResult = filteredOverviewResults;

            currentInstance.OverviewViewModel.AreSourcesEmpty = filteredOverviewResults.AbsenceEvents.Count == 0 &&
                filteredOverviewResults.AgendaEvents.Count == 0 &&
                filteredOverviewResults.Grades.Count == 0 &&
                filteredOverviewResults.Lessons.Count == 0;
            //to add notes check
        }


        public OverviewListView()
        {
            this.InitializeComponent();
        }


    }
}
