using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ClassevivaPCTO.Controls
{
    public sealed partial class GradesListView : UserControl
    {

        public List<Grade> ItemsSource
        {
            get { return (List<Grade>)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(List<Grade>),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GradesListView currentInstance = d as GradesListView;

            var newValue = e.NewValue as List<Grade>;

            var eventAdapters = newValue?.Select(evt => new GradeAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;
        }


        public GradesListView()
        {
            this.InitializeComponent();
        }


    }
}
