using System.Collections;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

namespace ClassevivaPCTO.Controls
{
    public static class KeepOldItemsSourceDecorator
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
            "ItemsSource", typeof(IEnumerable), typeof(KeepOldItemsSourceDecorator), new PropertyMetadata(null, ItemsSourcePropertyChanged)
        );

        public static void SetItemsSource(UIElement element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(UIElement element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        static void ItemsSourcePropertyChanged(DependencyObject element, 
                        DependencyPropertyChangedEventArgs e)
        {
            var target = element as Selector;
            if (target == null)
                return;

            var oldSelectedItem = target.SelectedItem;
            
            try
            {
                // we set the new itemssource...
                target.ItemsSource = e.NewValue as IEnumerable;

                // now we can set the old selected item
                target.SelectedItem = oldSelectedItem;
            }
            catch
            {
                // ignored
            }
        }
    }
}
