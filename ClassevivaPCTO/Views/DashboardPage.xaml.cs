using ClassevivaPCTO.Converters;
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Wrap;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ClassevivaPCTO.Views
{
    public sealed partial class DashboardPage : Page
    {
        private readonly IClassevivaAPI apiClient;

        private readonly IClassevivaAPI apiWrapper;

        public DashboardPageViewModel DashboardPageViewModel { get; } =
            new DashboardPageViewModel();

        public DashboardPage()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        //the app crashed with the error "Access is denied" because that class wasn't marked as "public"
        public class PoliciesDispatchProxy<T> : DispatchProxy
            where T : class, IClassevivaAPI
        {
            private T Target { get; set; }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                var retryPolicy = Policy
                    .Handle<AggregateException>()
                    .RetryAsync(
                        3,
                        async (exception, retryCount, context) =>
                        {
                            //we check whether the exception thrown is actually a Refit's ApiException
                            if (exception.InnerException is ApiException apiException)
                            {
                                if (
                                    apiException.StatusCode
                                    == System.Net.HttpStatusCode.Unauthorized
                                )
                                {
                                    //refresh token
                                    //await apiClient.RefreshTokenAsync();



                                    TaskCompletionSource<bool> IsSomethingLoading =
                                        new TaskCompletionSource<bool>();

                                    Debug.WriteLine("Test retry n.{0} policy ok ", retryCount);

                                    //the dispatcher.runasync method does not return any value, so actually the "await" is redundant, so to know when the dialog is done showing, we use the Taskcompletionsource hack
                                    await CoreApplication.MainView.Dispatcher.RunAsync(
                                        CoreDispatcherPriority.Normal,
                                        async () =>
                                        {
                                            ContentDialog noWifiDialog = new ContentDialog
                                            {
                                                Title = "Error",
                                                Content =
                                                    "Retry n."
                                                    + retryCount
                                                    + "\n"
                                                    + apiException.Message,
                                                CloseButtonText = "Ok"
                                            };

                                            ContentDialogResult result =
                                                await noWifiDialog.ShowAsync();

                                            IsSomethingLoading.SetResult(true);
                                        }
                                    );

                                    await IsSomethingLoading.Task;
                                }
                            }
                        }
                    );

                var fallback = Policy<object>
                    .Handle<Exception>()
                    .FallbackAsync(async ct =>
                    {
                        //if after the retries another exception occurs, then we let the call flow go ahead
                        return targetMethod.Invoke(Target, args);
                    });

                AsyncPolicyWrap<object> combinedpolicy = fallback.WrapAsync(retryPolicy);

                return combinedpolicy
                    .ExecuteAsync(async () =>
                    {
                        var lol = (targetMethod.Invoke(Target, args));

                        if (lol is Task task)
                        {
                            task.Wait(); //we wait for the result of the task, so we catch the exceptions if there are any
                        }

                        return lol; //if no exception occur then we return the result of the method call
                    })
                    .Result;
            }

            public static T CreateProxy(T target)
            {
                var proxy = Create<T, PoliciesDispatchProxy<T>>() as PoliciesDispatchProxy<T>;
                proxy.Target = target;
                return proxy as T;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            TextBenvenuto.Text =
                "Dashboard di " + VariousUtils.UppercaseFirst(cardResult.firstName);

            /*

            JsonConvert.DefaultSettings =
                       () => new JsonSerializerSettings()
                       {
                           Converters = { new CustomIntConverter() }
                       };

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new CustomIntConverter());

            */

            await Task.Run(async () =>
            {
                await LoadAgendaCard();
            });

            //run in a background thread otherwise the UI thread gets stuck when displaying a dialog
            await Task.Run(async () =>
            {
                await CaricaMediaCard();
            });
        }

        public async Task LoadAgendaCard()
        {
            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), loginResult.Token.ToString())
                .ConfigureAwait(false);

            var fiveMostRecent = result1.Grades.OrderByDescending(x => x.evtDate).Take(5);

            //update UI on UI thread
            await Window.Current.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    ListRecentGrades.ItemsSource = fiveMostRecent;

                    //todoo
                    ListViewAbsencesDate.ItemsSource = result1.Grades;
                    ListViewVotiDate.ItemsSource = result1.Grades;
                    ListViewAgendaDate.ItemsSource = result1.Grades;
                }
            );

        }

        public async Task CaricaMediaCard()
        {
            DashboardPageViewModel.IsLoadingMedia = true;

            LoginResult loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().CardsResult.Cards[0];

            var result1 = await apiWrapper
                .GetGrades(cardResult.usrId.ToString(), "asd")
                .ConfigureAwait(false);

            // Calcoliamo la media dei voti
            float media = VariousUtils.CalcolaMedia(result1.Grades);

            TextBlockMedia.Foreground = (Brush)
                new GradeToColorConverter().Convert(media, null, null, null);

            // Stampiamo la media dei voti
            TextBlockMedia.Text = media.ToString("0.00");
            TextBlockMedia.Visibility = Visibility.Visible;

            DashboardPageViewModel.IsLoadingMedia = false;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(typeof(Views.DettaglioVoti), null);
            NavigationService.Navigate(typeof(Views.DettaglioVoti));
        }

        private async void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Views.Agenda));
        }
    }
}
