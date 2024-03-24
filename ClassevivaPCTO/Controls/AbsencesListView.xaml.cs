using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using ClassevivaPCTO.Helpers;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class AbsencesListView : UserControl, INotifyPropertyChanged
    {
        public bool EnableEmptyAlert
        {
            get { return (bool) GetValue(EnableEmptyAlertProperty); }
            set { SetValue(EnableEmptyAlertProperty, value); }
        }

        private static readonly DependencyProperty EnableEmptyAlertProperty =
            DependencyProperty.Register(
                nameof(EnableEmptyAlert),
                typeof(bool),
                typeof(AbsencesListView),
                new PropertyMetadata(false, null));

        public bool EnableStickyHeader
        {
            get { return (bool) GetValue(EnableStickyHeaderProperty); }
            set { SetValue(EnableStickyHeaderProperty, value); }
        }

        private static readonly DependencyProperty EnableStickyHeaderProperty =
            DependencyProperty.Register(nameof(EnableStickyHeader), typeof(bool), typeof(AbsencesListView),
                new PropertyMetadata(false, null));

        private bool _showEmptyAlert;

        public bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            set { SetField(ref _showEmptyAlert, value); }
        }

        public List<AbsenceEvent> ItemsSource
        {
            get { return (List<AbsenceEvent>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<AbsenceEvent>),
                typeof(GradesListView),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private CollectionViewSource GroupedItems { get; set; }

        private static async Task<ObservableCollection<GroupInfoList>> GetAbsenceEventsGroupedAsync(
            IEnumerable<AbsenceEventAdapter> absenceEventAdapters)
        {
            var query = from item in absenceEventAdapters

                // Group the items returned from the query, sort and select the ones you want to keep
                group item by item.CurrentObject.evtCode
                into g
                orderby g.Key descending

                //TODO: forse ordinare ulteriormente ogni gruppo per data?

                //prendo il long name dell'enum con attributo ApiValueAttribute
                select new GroupInfoList(g) {Key = g.Key.ToString().GetLocalizedStr("plur")};

            return new ObservableCollection<GroupInfoList>(query);
        }

        private static async void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (AbsencesListView) d;

            var newValue = e.NewValue as List<AbsenceEvent>;

            var eventAdapters = newValue?.Select(evt => new AbsenceEventAdapter(evt)).ToList();

            currentInstance.GroupedItems = new CollectionViewSource
            {
                IsSourceGrouped = currentInstance.EnableStickyHeader, //TODO: settare proprietà da dependencyproperty

                //in base al valore di IsSourceGrouped, Source può essere un IEnumerable oppure un IList
                Source = currentInstance.EnableStickyHeader
                    ? await GetAbsenceEventsGroupedAsync(eventAdapters)
                    : eventAdapters
            };

            //update the listview contents
            currentInstance.MainListView.ItemsSource = currentInstance.GroupedItems.View;
            
            //reset the selection
            currentInstance.MainListView.SelectedIndex = -1;

            currentInstance.ShowEmptyAlert =
                (newValue == null || newValue.Count == 0) && currentInstance.EnableEmptyAlert;
        }

        public AbsencesListView()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}