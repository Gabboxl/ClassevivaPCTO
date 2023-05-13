﻿using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Controls;

namespace ClassevivaPCTO.Views
{
    public sealed partial class Bacheca : Page
    {
        public BachecaViewModel BachecaViewModel { get; } = new BachecaViewModel();

        private readonly IClassevivaAPI apiWrapper;

        public Bacheca()
        {
            this.InitializeComponent();

            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            BachecaViewModel.IsLoadingBacheca = true;

            this.NoticesListView.OnShouldUpdate += OnShouldUpdate; 

            await Task.Run(async () =>
            {
                await LoadData();
            });
        }

        private async void OnShouldUpdate(object sender, EventArgs args)
        {
            await Task.Run(async () => { await LoadData(); });
        }

        private async Task LoadData()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    BachecaViewModel.IsLoadingBacheca = true;
                }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;


            NoticeboardResult noticeboardResult = await apiWrapper.GetNotices(
                cardResult.usrId.ToString(),
                loginResult.token
            );


            var noticeAdapters = noticeboardResult.Notices?.Select(evt => new NoticeAdapter(evt)).ToList();

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    NoticesListView.ItemsSource = noticeboardResult.Notices;

                    BachecaViewModel.IsLoadingBacheca = false;
                }
            );

        }

        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                await LoadData();
            });
        }
    }
}
