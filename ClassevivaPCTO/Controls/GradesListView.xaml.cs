using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.Controls
{
    [INotifyPropertyChanged]
    public sealed partial class GradesListView : UserControl
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
                typeof(GradesListView),
                new PropertyMetadata(false, null));

        private bool _showEmptyAlert;

        private bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            set { SetProperty(ref _showEmptyAlert, value); }
        }

        public List<Grade> ItemsSource
        {
            get { return (List<Grade>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(List<Grade>),
                typeof(GradesListView),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (GradesListView) d;

            var newValue = e.NewValue as List<Grade>;

            var eventAdapters = newValue?.Select(evt => new GradeAdapter(evt)).ToList();

            currentInstance.MainListView.ItemsSource = eventAdapters;

            currentInstance.ShowEmptyAlert =
                (newValue == null || newValue.Count == 0) && currentInstance.EnableEmptyAlert;
        }

        public GradesListView()
        {
            InitializeComponent();
        }
    }
}