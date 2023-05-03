using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using System.Collections.Generic;
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

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(OverviewDataModel),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = d as OverviewListView;

            var newValue = e.NewValue as OverviewDataModel;

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
                            agendaEvent.evtDatetimeBegin <= newValue.FilterDate.Date && agendaEvent.evtDatetimeEnd >= newValue.FilterDate.Date
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
        }


        public OverviewListView()
        {
            this.InitializeComponent();
        }


    }
}
