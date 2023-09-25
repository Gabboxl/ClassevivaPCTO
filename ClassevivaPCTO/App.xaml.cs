using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Views;

namespace ClassevivaPCTO
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        private async Task<string> GetTokenAsync()
        {
            LoginResultComplete? loginResult = ViewModelHolder.GetViewModel().LoginResult;

            return loginResult.token;
        }


        public IServiceProvider Container { get; }

        private IServiceProvider ConfigureDependencyInjection()
        {
            var getToken = new Func<Task<string>>(GetTokenAsync);


            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddRefitClient(typeof(IClassevivaAPI))
                .ConfigureHttpClient(
                    (sp, client) => { client.BaseAddress = new Uri(Endpoint.CurrentEndpoint); }
                )
                .AddHttpMessageHandler(() => new AuthenticatedHttpClientHandler(getToken));

            return serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Inizializza l'oggetto Application singleton. Si tratta della prima riga del codice creato
        /// creato e, come tale, corrisponde all'equivalente logico di main() o WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            Container = ConfigureDependencyInjection();

            UnhandledException += OnAppUnhandledException;

            bool isDebugMode = false;

#if DEBUG
            isDebugMode = true;
#endif

            if (!isDebugMode)
            {
                Microsoft.AppCenter.AppCenter.Start(
                    "test",
                    typeof(Microsoft.AppCenter.Analytics.Analytics),
                    typeof(Microsoft.AppCenter.Crashes.Crashes)
                );

                //Microsoft.AppCenter.Analytics.Analytics.TrackEvent("App started");
            }

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);


/*

            CoreApplication.UnhandledErrorDetected += (sender, eventArgs) =>
            {
                try
                {
                    eventArgs.UnhandledError.Propagate();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("OKBRO:" + ex.StackTrace);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    
                }
            };
            */
        }

        /// <summary>
        /// Richiamato quando l'applicazione viene avviata normalmente dall'utente finale. All'avvio dell'applicazione
        /// verranno usati altri punti di ingresso per aprire un file specifico.
        /// </summary>
        /// <param name="e">Dettagli sulla richiesta e sul processo di avvio.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PrelaunchActivated == false)
            {
                await ActivationService.ActivateAsync(e);
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Richiamato quando l'esecuzione dell'applicazione viene sospesa. Lo stato dell'applicazione viene salvato
        /// senza che sia noto se l'applicazione verrà terminata o ripresa con il contenuto
        /// della memoria ancora integro.
        /// </summary>
        /// <param name="sender">Origine della richiesta di sospensione.</param>
        /// <param name="e">Dettagli relativi alla richiesta di sospensione.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: salvare lo stato dell'applicazione e arrestare eventuali attività eseguite in background
            deferral.Complete();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(
            object sender,
            Windows.UI.Xaml.UnhandledExceptionEventArgs e
        )
        {
            //http://blog.wpdev.fr/inspecting-unhandled-exceptions-youve-got-only-one-chance/
            Exception exceptionThatDoesntGoAway = e.Exception;

            //create a list of ErrorAttachmentLog
            var attachments = new List<ErrorAttachmentLog>();

            try
            {
                var dataLogin = ViewModelHolder.GetViewModel().LoginResult;

                var serializedLogin = Newtonsoft.Json.JsonConvert.SerializeObject(
                    dataLogin,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                    }
                );

                var er2 = ErrorAttachmentLog.AttachmentWithText(serializedLogin, "dataLogin.txt");
                attachments.Add(er2);
            }
            catch (Exception ex)
            {
            }

            Debug.WriteLine("TESTEXCEPTION: " + exceptionThatDoesntGoAway.StackTrace);
            Trace.WriteLine("SIDE: " + exceptionThatDoesntGoAway.StackTraceEx());
            Trace.WriteLine("SIDE2: " + exceptionThatDoesntGoAway.StackTrace);

            e.Handled = true;

            Crashes.TrackError(exceptionThatDoesntGoAway, attachments: attachments.ToArray());

            //visualizzare un dialog con si è verificato un errore
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(LoginPage), null);
        }

        /*private UIElement CreateShell()
        {
            return new Views.ShellPage();
        } */
    }
}