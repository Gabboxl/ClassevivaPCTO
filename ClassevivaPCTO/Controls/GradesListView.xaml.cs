using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
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

        public List<GradeAdapter> ItemsSource
        {
            get { return (List<GradeAdapter>)GetValue(ItemsSourceProperty); }
            set {
                listView.ItemsSource = value;
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(List<GradeAdapter>),
                typeof(GradesListView),
                new PropertyMetadata(null));


        public GradesListView()
        {
            this.InitializeComponent();
        }


    }
}
