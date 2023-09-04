using Polly;
using Polly.Wrap;
using Refit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ClassevivaPCTO.Views;
using Windows.UI.Xaml.Media.Animation;
using ClassevivaPCTO.ViewModels;
using System.Threading;

namespace ClassevivaPCTO.Utils
{
    //the app crashed with the error "Access is denied" because that class wasn't marked as "public"
    public class PoliciesDispatchProxy<T> : DispatchProxy
        where T : class, IClassevivaAPI
    {
        private T Target { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            // Create a CancellationTokenSource
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            int retryCount = 3;
            int currentRetryAttempts = 0;

            var retryPolicy = Policy
                .Handle<AggregateException>()
                .RetryAsync(
                    retryCount,
                    async (exception, retryCount, context) =>
                    {
                        currentRetryAttempts = retryCount;

                        //we check whether the exception thrown is actually a Refit's ApiException
                        if (exception.InnerException is ApiException apiException)
                        {
                            if (
                                apiException.StatusCode == System.Net.HttpStatusCode.Unauthorized
                            )
                            {
                                //TODO: refresh token
                                //await apiClient.RefreshTokenAsync();

                                Debug.WriteLine("Test retry n.{0} policy ok ", retryCount);


                                var loginCredentials = new CredUtils().GetCredentialFromLocker();

                                if (loginCredentials == null)
                                {
                                    TaskCompletionSource<bool> isSomethingLoading =
                                        new TaskCompletionSource<bool>();

                                    //the dispatcher.runasync method does not return any value, so actually the "await" is redundant, so to know when the dialog is done showing, we use the Taskcompletionsource hack
                                    await CoreApplication.MainView.Dispatcher.RunAsync(
                                        CoreDispatcherPriority.Normal,
                                        async () =>
                                        {
                                            ContentDialog noWifiDialog = new ContentDialog
                                            {
                                                Title = "Sessione scaduta",
                                                Content =
                                                    "Una sessione dura un'ora dal login. Non avendo salvato le credenziali, ora verrai portato alla pagina di login. ",
                                                PrimaryButtonText = "Ok"
                                            };

                                            ContentDialogResult result =
                                                await noWifiDialog.ShowAsync();

                                            //go to login page
                                            Frame rootFrame = (Frame) Window.Current.Content;
                                            rootFrame.Navigate(
                                                typeof(LoginPage),
                                                null,
                                                new DrillInNavigationTransitionInfo()
                                            );


                                            isSomethingLoading.SetResult(true);
                                        }
                                    );

                                    //we discard the remaining retries
                                    cancellationTokenSource.Cancel();

                                    await isSomethingLoading.Task;
                                }
                                else
                                {
                                    //refresho il token

                                    loginCredentials
                                        .RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota

                                    //get app viewmodel from holder
                                    var appViewModel = ViewModelHolder.GetViewModel();

                                    var refreshLoginData = new LoginData
                                    {
                                        Uid = loginCredentials.UserName,
                                        Pass = loginCredentials.Password,
                                        Ident = appViewModel.SingleCardResult.ident
                                    };

                                    try
                                    {
                                        LoginResultComplete? loginResult =
                                            (LoginResultComplete) await CvUtils.GetApiLoginData(
                                                Target,
                                                refreshLoginData
                                            );

                                        //salvo il nuovo token
                                        ViewModelHolder.GetViewModel().LoginResult = loginResult;
                                    }
                                    catch (Exception e)
                                    {
                                        TaskCompletionSource<bool> isSomethingLoading =
                                            new TaskCompletionSource<bool>();

                                        //the dispatcher.runasync method does not return any value, so actually the "await" is redundant, so to know when the dialog is done showing, we use the Taskcompletionsource hack
                                        await CoreApplication.MainView.Dispatcher.RunAsync(
                                            CoreDispatcherPriority.Normal,
                                            async () =>
                                            {
                                                ContentDialog noWifiDialog = new ContentDialog
                                                {
                                                    Title = "Errore login",
                                                    Content =
                                                        "Non è stato possibile effettuare il login. Riprova più tardi.",
                                                    CloseButtonText = "Ok"
                                                };

                                                ContentDialogResult result =
                                                    await noWifiDialog.ShowAsync();


                                                //go to login page
                                                Frame rootFrame = (Frame) Window.Current.Content;
                                                rootFrame.Navigate(
                                                    typeof(LoginPage),
                                                    null,
                                                    new DrillInNavigationTransitionInfo()
                                                );


                                                isSomethingLoading.SetResult(true);
                                            }
                                        );

                                        //we discard the remaining retries
                                        cancellationTokenSource.Cancel();

                                        await isSomethingLoading.Task;
                                    }
                                }
                            }
                        }
                    }
                );


            var policyResult = retryPolicy
                .ExecuteAndCaptureAsync(async (ct) =>
                {
                    var result = targetMethod.Invoke(Target, args);

                    if (result is Task task)
                    {
                        //await task.ConfigureAwait(false);
                        task.Wait(); //we wait for the result of the task, so we catch the exceptions if there are any
                    }

                    return result; //if no exception occur then we return the result of the method call
                }, cancellationTokenSource.Token).Result;


            if (policyResult.Outcome == OutcomeType.Failure)
            {
                //var lol = result.FinalException.InnerException.GetType();
                //var targetReturnType = targetMethod.ReturnType;


                if (policyResult.FinalException.InnerException is ApiException apiException)
                {
                    //if (apiException.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)

                    TaskCompletionSource<bool> isSomethingLoading =
                        new TaskCompletionSource<bool>();


                    CoreApplication.MainView.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        async () =>
                        {
                            ContentDialog noWifiDialog = new ContentDialog
                            {
                                Title = "Errore chiamata API (riprova più tardi)",
                                Content =
                                    "Tentativi effettuati: "
                                    + currentRetryAttempts
                                    + "\n"
                                    + apiException.Message,
                                CloseButtonText = "Ok"
                            };

                            try
                            {
                                ContentDialogResult result =
                                    await noWifiDialog.ShowAsync();
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Exception in dispatcher: " + e.Message);
                            }

                            isSomethingLoading.SetResult(true);
                        }
                    );

                    var task = isSomethingLoading.Task;


                    var result = targetMethod.Invoke(Target, args);

                    return result;
                }
            }

            return policyResult.Result; // On success, return the result of the action
        }

        public static T CreateProxy(T target)
        {
            var proxy = Create<T, PoliciesDispatchProxy<T>>() as PoliciesDispatchProxy<T>;
            proxy.Target = target;
            return proxy as T;
        }
    }
}