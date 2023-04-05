using Polly;
using Refit;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml.Controls;


namespace ClassevivaPCTO.Views
{
    public sealed partial class DashboardPage
    {
        public class ApiPolicyWrapper<T> where T : class
        {
            private readonly T _api;

            public ApiPolicyWrapper(T api)
            {
                _api = api;
            }

            public async Task<TResult> CallApi<TResult>(Func<T, Task<TResult>> apiCall)
            {

                DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

                var policy = Policy
                    .Handle<ApiException>()
                    .RetryAsync(2,
                            async (ex, count) =>
                            {

                                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                       Windows.UI.Core.CoreDispatcherPriority.Normal,
                                       async () =>
                                       {
                                           ContentDialog dialog = new ContentDialog();
                                           dialog.Title = "ApiException";
                                           dialog.PrimaryButtonText = "OK";
                                           dialog.DefaultButton = ContentDialogButton.Primary;
                                           dialog.Content =
                                               "Errore: " + ex.Message;

                                           try
                                           {
                                               var result = await dialog.ShowAsync();
                                           }
                                           catch (Exception e)
                                           {
                                               System.Console.WriteLine(e.ToString());
                                           }
                                       }
                                   );


                            }
                                );

                return await policy.ExecuteAsync(async () => await apiCall(_api));
            }


        }
    }
}
