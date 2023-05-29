using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Force.DeepCloner;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class AgendaListView : UserControl
    {

        public bool IsMultipleDaysList
        {
            get { return (bool)GetValue(IsMultipleDaysListProperty); }
            set
            {
                SetValue(IsMultipleDaysListProperty, value);
            }
        }

        private static readonly DependencyProperty IsMultipleDaysListProperty =
            DependencyProperty.Register(
                nameof(IsMultipleDaysList),
                typeof(bool),
                typeof(LessonsListView),
                new PropertyMetadata(false, null));

        public List<AgendaEvent> ItemsSource
        {
            get { return (List<AgendaEvent>)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<AgendaEvent>),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (AgendaListView)d;

            var newValue = e.NewValue as List<AgendaEvent>;

            var orderedAgendaEvents = newValue;

            if (currentInstance.IsMultipleDaysList)
            {
                //repeat days whose startdate and enddate span multiple days in the list

                var repeatedDays = new List<AgendaEvent>();
                foreach (var currentEvt in orderedAgendaEvents.ToList())
                {
                    if (currentEvt.evtDatetimeEnd.Date > currentEvt.evtDatetimeBegin.Date.AddDays(1))
                    {
                        var days = (int)(currentEvt.evtDatetimeEnd.Date - currentEvt.evtDatetimeBegin.Date).TotalDays + 1;

                        for (int i = 0; i < days; i++)
                        {
                            var clonedDay = currentEvt.DeepClone();

                            clonedDay.evtDatetimeBegin = currentEvt.evtDatetimeBegin.AddDays(i);

                            clonedDay.evtDatetimeEnd = currentEvt.evtDatetimeEnd.Date.AddDays(i);

                            //add the new event to the list
                            repeatedDays.Add(clonedDay);

                            //remove the original event
                            orderedAgendaEvents.Remove(currentEvt);
                        }
                        continue;
                    }

                    repeatedDays.Add(currentEvt);
                }

                orderedAgendaEvents = repeatedDays.ToList(); //we need to reorder the lissttt
            }


            var eventAdapters = orderedAgendaEvents?.Select(evt => new AgendaEventAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;
        }


        public AgendaListView()
        {
            this.InitializeComponent();
        }


    }
}
