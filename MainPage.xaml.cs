using System;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;

namespace ClassevivaPCTO
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            Window.Current.SetTitleBar(AppTitleBar);


            //remove the solid-colored backgrounds behind the caption controls and system back button
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;


            nvSample.BackRequested += new Windows.Foundation.TypedEventHandler<Microsoft.UI.Xaml.Controls.NavigationView, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs>((Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args) => { NavView_BackRequested(sender, args); });
            contentFrame.Navigated += On_Navigated;
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                if (contentFrame.CurrentSourcePageType != typeof(DashboardPage))
                {
                    contentFrame.Navigate(typeof(DashboardPage));
                }
            }
            else
            {
                var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
                if (selectedItem != null)
                {
                    string selectedItemTag = ((string)selectedItem.Tag);

                    string pageName = "ClassevivaPCTO." + selectedItemTag;
                    Type pageType = Type.GetType(pageName);
                    contentFrame.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
            }

        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            
            nvSample.IsBackEnabled = contentFrame.CanGoBack;
            nvSample.IsBackButtonVisible = contentFrame.CanGoBack ? Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Visible : Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible.Collapsed;

            /* if (contentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
                NavView.Header = "Settings";
            }
            else  */
            if (contentFrame.SourcePageType != null)
            {
                
                //nvSample.SelectedItem = e.SourcePageType;

                nvSample.Header =
                    ((Microsoft.UI.Xaml.Controls.NavigationViewItem)nvSample.SelectedItem)?.Content?.ToString();
            }
        }






            private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender,
                               Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!contentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (nvSample.IsPaneOpen &&
                (nvSample.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Compact ||
                 nvSample.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal))
                return false;

            contentFrame.GoBack();
            return true;
        }
    }



}
