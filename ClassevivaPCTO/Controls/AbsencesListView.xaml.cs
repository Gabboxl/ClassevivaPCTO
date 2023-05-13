using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class AbsencesListView : UserControl
    {

        public List<AbsenceEvent> ItemsSource
        {
            get { return (List<AbsenceEvent>)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<AbsenceEvent>),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (AbsencesListView)d;

            var newValue = e.NewValue as List<AbsenceEvent>;

            var eventAdapters = newValue?.Select(evt => new AbsenceEventAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;
        }


        public AbsencesListView()
        {
            this.InitializeComponent();
        }


    }
}
