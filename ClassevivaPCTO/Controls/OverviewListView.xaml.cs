using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class OverviewListView : UserControl
    {

        public OverviewResult ItemsSource
        {
            get { return (OverviewResult)GetValue(ItemsSourceProperty); }
            set {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(OverviewResult),
                typeof(GradesListView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = d as OverviewListView;

            var newValue = e.NewValue as OverviewResult;
            
            currentInstance.AbsencesListView.ItemsSource = newValue.AbsenceEvents;
            currentInstance.GradesListView.ItemsSource = newValue.Grades;
            currentInstance.LessonsListView.ItemsSource = newValue.Lessons;
            currentInstance.AgendaListView.ItemsSource = newValue.AgendaEvents;
            //listview note da mettere

            //da mettere controllo che se è vuoto visualizza un testo enorme
        }


        public OverviewListView()
        {
            this.InitializeComponent();
        }


    }
}
