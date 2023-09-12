using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Controls
{
    public sealed partial class GradesListView : UserControl, INotifyPropertyChanged
    {
        public bool EnableEmptyAlert
        {
            get { return (bool) GetValue(EnableEmptyAlertProperty); }
            set
            {
                SetValue(EnableEmptyAlertProperty, value);
                _showEmptyAlert = value;
            }
        }

        private static readonly DependencyProperty EnableEmptyAlertProperty =
            DependencyProperty.Register(
                nameof(EnableEmptyAlert),
                typeof(bool),
                typeof(GradesListView),
                new PropertyMetadata(false, null));

        private static bool _showEmptyAlert;

        public bool ShowEmptyAlert
        {
            get { return _showEmptyAlert; }
            set { SetField(ref _showEmptyAlert, value); }
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
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentInstance = (GradesListView) d;

            var newValue = e.NewValue as List<Grade>;

            var eventAdapters = newValue?.Select(evt => new GradeAdapter(evt)).ToList();

            currentInstance.listView.ItemsSource = eventAdapters;


            currentInstance.ShowEmptyAlert = (newValue == null || newValue.Count == 0) && _showEmptyAlert;
        }


        public GradesListView()
        {
            this.InitializeComponent();
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