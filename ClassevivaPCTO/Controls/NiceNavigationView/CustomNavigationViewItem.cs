using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;

namespace ClassevivaPCTO.Controls.NiceNavigationView
{
    public class CustomNavigationViewItem : NavigationViewItem
    {
        public static readonly DependencyProperty IconOutlineProperty =
            DependencyProperty.Register(nameof(IconOutline), typeof(IconElement), typeof(CustomNavigationViewItem),
                new PropertyMetadata(null));

        public IconElement IconOutline
        {
            get { return (IconElement) GetValue(IconOutlineProperty); }
            set { SetValue(IconOutlineProperty, value); }
        }

        public static readonly DependencyProperty IconFilledProperty =
            DependencyProperty.Register(nameof(IconFilled), typeof(IconElement), typeof(CustomNavigationViewItem),
                new PropertyMetadata(null));

        public IconElement IconFilled
        {
            get { return (IconElement) GetValue(IconFilledProperty); }
            set { SetValue(IconFilledProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetIcon();

            RegisterPropertyChangedCallback(IsSelectedProperty, OnIsSelectedChanged);
        }

        private void OnIsSelectedChanged(DependencyObject sender, DependencyProperty dp)
        {
            SetIcon();
        }

        private void SetIcon()
        {
            if ((bool) GetValue(IsSelectedProperty))
            {
                SetValue(IconProperty, IconFilled);
            }
            else
            {
                SetValue(IconProperty, IconOutline);
            }
        }
    }
}