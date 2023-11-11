using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.WinUI.Controls;

namespace ClassevivaPCTO.Controls
{
    public class CustomStaggeredPanel : StaggeredPanel
    {
        private const double MaxColumnWidth = 500;
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    // Calculate the number of columns based on the max column width
        //    int columns = (int)(availableSize.Width / MaxColumnWidth);
        //    // Set the desired column width
        //    this.DesiredColumnWidth = availableSize.Width / columns;
        //    return base.MeasureOverride(availableSize);
        //}
    }
}