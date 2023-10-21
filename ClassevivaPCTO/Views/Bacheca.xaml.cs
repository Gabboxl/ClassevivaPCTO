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

namespace ClassevivaPCTO.Views
{
    public sealed partial class Bacheca : Page
    {
        public BachecaViewModel BachecaViewModel { get; } = new();

        private readonly IClassevivaAPI apiWrapper;

        public Bacheca()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            BachecaViewModel.IsLoadingBacheca = true;

            this.NoticesListView.OnShouldUpdate += OnShouldUpdate;

            CheckboxAttive.Checked += AggiornaCommand_Click;
            CheckboxAttive.Unchecked += AggiornaCommand_Click;

            await Task.Run(async () => { await LoadData(); });
        }

        private async void OnShouldUpdate(object sender, EventArgs args)
        {
            await Task.Run(async () => { await LoadData(); });
        }

        private async Task LoadData()
        {
            try
            {
                bool showInactiveNotices = false;
                int readUnreadSegmentedIndex = 0;

                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        BachecaViewModel.IsLoadingBacheca = true;

                        readUnreadSegmentedIndex = ReadUnreadSegmented.SelectedIndex;

                        if (CheckboxAttive.IsChecked != null) showInactiveNotices = CheckboxAttive.IsChecked.Value;
                    }
                );

                Card? cardResult = ViewModelHolder.GetViewModel().SingleCardResult;


                NoticeboardResult noticeboardResult = await apiWrapper.GetNotices(
                    cardResult.usrId.ToString()
                );

                var noticesToShow = noticeboardResult.Notices;

                if (!showInactiveNotices) //there are also notes that are deleted but still active (not expired), so we filter them out too
                {
                    //filter notices that are valid from the cntValidInRange property
                    noticesToShow = noticesToShow.Where(n => n.cntValidInRange && n.cntStatus != "deleted").ToList();
                }

                //filter notices by read status
                switch (readUnreadSegmentedIndex)
                {
                    case 1:
                        noticesToShow = noticesToShow.Where(n => !n.readStatus).ToList();
                        break;

                    case 2:
                        noticesToShow = noticesToShow.Where(n => n.readStatus).ToList();
                        break;

                    default:
                        break;
                }

                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        //set the notices to show
                        BachecaViewModel.NoticesToShow = noticesToShow;
                    }
                );
            }
            finally
            {
                {
                    await CoreApplication.MainView.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        async () => { BachecaViewModel.IsLoadingBacheca = false; }
                    );
                }
            }
        }

        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(async () => { await LoadData(); });
        }

        private async void ReadUnreadSegmented_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Run(async () => { await LoadData(); });
        }
    }
}