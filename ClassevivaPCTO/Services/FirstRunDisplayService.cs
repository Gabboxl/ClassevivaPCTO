using ClassevivaPCTO.Views;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace ClassevivaPCTO.Services
{
    public static class FirstRunDisplayService
    {
        private static bool shown = false;

        internal static async Task ShowIfAppropriateAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    //if (ApplicationDeployment.CurrentDeployment.isFirstRun && !shown)
                    //{
                        shown = true;
                        var dialog = new FirstRunDialog();
                        await dialog.ShowAsync();
                    //}
                });
        }
    }
}
