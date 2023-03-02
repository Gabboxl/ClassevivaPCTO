
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace ClassevivaPCTO.Controls
{
    public sealed partial class ProfileFlyoutControl : UserControl
    {


        public static readonly DependencyProperty FlyoutBaseProperty =
            DependencyProperty.Register(
                nameof(FlyoutBase),
                typeof(FlyoutBase),
                typeof(ProfileFlyoutControl),
                null);

        public FlyoutBase FlyoutBase
        {
            get => (FlyoutBase)GetValue(FlyoutBaseProperty);
            set => SetValue(FlyoutBaseProperty, value);
        }

    }
}
