using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using CloneExtensions;

namespace ClassevivaPCTO.Controls
{
    public class GroupInfoList : List<object>
    {
        public GroupInfoList(IEnumerable<object> items) : base(items)
        {
        }


        public object Key { get; set; }
    }

    public sealed partial class AgendaMultipleDaysListView : UserControl
    {
        public CollectionViewSource GroupedItems { get; set; }


        public static async Task<ObservableCollection<GroupInfoList>> GetContactsGroupedAsync(
            List<AgendaEventAdapter> agendaEvents)
        {
            var query = from item in agendaEvents

                // Group the items returned from the query, sort and select the ones you want to keep
                group item by item.CurrentObject.evtDatetimeBegin.Date
                into g
                orderby g.Key descending

                //ricorda di trasformare il datetime object in string per la Key siccome in XAML viene visualizzato un oggetto "object" senza ToString()
                select new GroupInfoList(g) {Key = g.Key.Date.ToShortDateString()};

            return new ObservableCollection<GroupInfoList>(query);
        }


        public List<AgendaEvent> ItemsSource
        {
            get { return (List<AgendaEvent>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<AgendaEvent>),
                typeof(AgendaMultipleDaysListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (AgendaMultipleDaysListView) d;

            var newValue = e.NewValue as List<AgendaEvent>;

            var orderedAgendaEvents = newValue;


            {
                //repeat days whose startdate and enddate span multiple days in the list
                var repeatedDays = new List<AgendaEvent>();
                foreach (var currentEvt in orderedAgendaEvents.ToList())
                {
                    if (currentEvt.evtDatetimeEnd.Date > currentEvt.evtDatetimeBegin.Date.AddDays(1))
                    {
                        int days = (int) (currentEvt.evtDatetimeEnd.Date - currentEvt.evtDatetimeBegin.Date).TotalDays + 1;

                        for (int i = 0; i < days; i++)
                        {
                            var clonedDay = currentEvt.GetClone();

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

            var asd = GetContactsGroupedAsync(eventAdapters);

            currentInstance.GroupedItems = new CollectionViewSource
            {
                IsSourceGrouped = true,
                Source = asd.Result
            };


            currentInstance.listView.ItemsSource = currentInstance.GroupedItems.View;
        }


        public AgendaMultipleDaysListView()
        {
            this.InitializeComponent();
        }
    }
}