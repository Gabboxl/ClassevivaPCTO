
using ClassevivaPCTO.Services;
using ClassevivaPCTO.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Polly;
using Polly.Registry;
using Polly.Retry;
using Refit;
using System;
using System.Runtime.CompilerServices;
using WinUIEx;

namespace ClassevivaPCTO
{
    public partial class App : Application
    {
        // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        public IHost Host
        {
            get;
        }

        public static T GetService<T>()
            where T : class
        {
            if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
            }

            return service;
        }

        public static WindowEx Window { get; } = new MainWindow();

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.
            CreateDefaultBuilder().
            UseContentRoot(AppContext.BaseDirectory).
            ConfigureServices((context, services) =>
            {
                /*
                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers

                // Services
                services.AddTransient<INavigationViewService, NavigationViewService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Core Services
                services.AddSingleton<IFileService, FileService>();

                // Views and ViewModels
                services.AddTransient<BlankViewModel>();
                services.AddTransient<BlankPage>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                */

                /*
                // 1. Define your Polly policy
                var retryPolicy = Policy.Handle<ApiException>()
                                        .WaitAndRetryAsync(new[]
                                        {
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(2),
                            TimeSpan.FromSeconds(3)
                                        });


                var refitSettings = new RefitSettings
                {
                    HttpMessageHandlerFactory = () => new PollyHandler(retryPolicy)
                };

                // 3. Configure the Refit client to use the HttpClient instance with the PolicyHttpMessageHandler added
                //var refitClient = RestService.For<IClassevivaAPI>(Endpoint.CurrentEndpoint);

                */

                //services.AddTransient<PollyContextInjectingDelegatingHandler>();




                services.AddRefitClient(typeof(IClassevivaAPI))
                .ConfigureHttpClient(
    (sp, client) =>
    {
        client.BaseAddress = new Uri(Endpoint.CurrentEndpoint);

    }
);


                // Configuration
            }).
            Build();

            UnhandledException += App_UnhandledException;
        }





        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            Window.Activate();//darimv

            //await App.GetService<IActivationService>().ActivateAsync(args);
        }
    }

    }
