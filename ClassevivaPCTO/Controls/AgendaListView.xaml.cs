using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class AgendaListView : UserControl
    {

        public List<AgendaEvent> ItemsSource
        {
            get { return (List<AgendaEvent>)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<AgendaEvent>),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (AgendaListView)d;

            var newValue = e.NewValue as List<AgendaEvent>;

            var eventAdapters = newValue?.Select(evt => new AgendaEventAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;
        }


        public AgendaListView()
        {
            this.InitializeComponent();
        }


    }
}
