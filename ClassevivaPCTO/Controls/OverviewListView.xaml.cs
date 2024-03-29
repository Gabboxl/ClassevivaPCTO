﻿using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ClassevivaPCTO.DataModels;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class OverviewListView : UserControl
    {
        public OverviewControlViewModel OverviewControlViewModel { get; } = new();

        public OverviewDataModel ItemsSource
        {
            get { return (OverviewDataModel) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(OverviewDataModel),
                typeof(GradesListView),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (OverviewListView) d;

            var newValue = (OverviewDataModel) e.NewValue;

            currentInstance.OverviewControlViewModel.CurrentOverviewData = newValue;

            //OverviewResult overviewResult = newValue.OverviewData;
            OverviewResult overviewResult = currentInstance.OverviewControlViewModel.CurrentOverviewData.OverviewData;

            OverviewResult filteredOverviewResults = overviewResult;

            if (newValue.FilterDate != null)
            {
                //filteredOverviewResults = new OverviewResult();
                filteredOverviewResults.AbsenceEvents = overviewResult.AbsenceEvents
                    .Where(abs => abs.evtDate.Date == newValue.FilterDate.Date).ToList();

                //filter agenda events if the selected date is between the start and end date of the event
                filteredOverviewResults.AgendaEvents = overviewResult.AgendaEvents
                    .Where(
                        agendaEvent =>
                            agendaEvent.evtDatetimeBegin.Date <= newValue.FilterDate.Date &&
                            agendaEvent.evtDatetimeEnd.Date >=
                            newValue.FilterDate
                                .Date //be sure to use the Date property instead of the DateTime one because the time is not important and there is risk some oneday-only events are not shown
                    )
                    .ToList();

                filteredOverviewResults.Grades = overviewResult.Grades
                    .Where(grade => grade.evtDate == newValue.FilterDate.Date).ToList();

                filteredOverviewResults.Notes =
                    overviewResult.Notes.Where(note => note.evtDate == newValue.FilterDate.Date).ToList();

                //questo per le lezioni in teoria non serve
                //filteredOverviewResults.Lessons = overviewResult.Lessons.Where(les => les.evtDate == newValue.FilterDate.Date).ToList();
            }

            //set the data to the listviews

            currentInstance.AbsencesListView.ItemsSource = filteredOverviewResults.AbsenceEvents;
            currentInstance.GradesListView.ItemsSource = filteredOverviewResults.Grades;
            currentInstance.LessonsListView.ItemsSource = filteredOverviewResults.Lessons;
            currentInstance.AgendaListView.ItemsSource = filteredOverviewResults.AgendaEvents;
            currentInstance.NotesListView.ItemsSource = filteredOverviewResults.Notes;
            currentInstance.OverviewControlViewModel.FilteredOverviewResult = filteredOverviewResults;

            currentInstance.OverviewControlViewModel.ShowEmptyAlert =
                filteredOverviewResults.AbsenceEvents.Count == 0 &&
                filteredOverviewResults.AgendaEvents.Count == 0 &&
                filteredOverviewResults.Grades.Count == 0 &&
                filteredOverviewResults.Lessons.Count == 0 &&
                filteredOverviewResults.Notes.Count == 0;
        }

        public OverviewListView()
        {
            InitializeComponent();
        }
    }
}