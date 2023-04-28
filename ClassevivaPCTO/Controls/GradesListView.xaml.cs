using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            GradesListView asd = d as GradesListView;

            var newValue = e.NewValue as List<Grade>;

            var eventAdapters = newValue?.Select(evt => new GradeAdapter(evt)).ToList();

            asd.listView.ItemsSource = eventAdapters;
        }


        public GradesListView()
        {
            this.InitializeComponent();
        }


    }
}
