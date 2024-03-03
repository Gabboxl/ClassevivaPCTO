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
    public sealed partial class BachecaPage : CustomAppPage
    {
        public BachecaViewModel BachecaViewModel { get; } = new();

        private readonly IClassevivaAPI apiWrapper;

        public BachecaPage()
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

            CheckboxAttive.Checked += (sender, args) => { AggiornaAction(); }; 
            CheckboxAttive.Unchecked += (sender, args) => { AggiornaAction(); };

            await Task.Run(async () => { await LoadData(); });
        }

        private async Task LoadData()
        {
            try
            {
                bool showInactiveNotices = false;
                int readUnreadSegmentedIndex = 0;
                string? selectedCategory = null;

                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        BachecaViewModel.IsLoadingBacheca = true;

                        selectedCategory = (string) CategoryComboBox.SelectionBoxItem;

                        readUnreadSegmentedIndex = ReadUnreadSegmented.SelectedIndex;

                        if (CheckboxAttive.IsChecked != null) showInactiveNotices = CheckboxAttive.IsChecked.Value;
                    }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

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
                }

                var noticeCategories = noticesToShow.Select(n => n.cntCategory).Distinct().OrderBy(o => o).ToList();

                if (!string.IsNullOrEmpty(selectedCategory))
                {
                    noticesToShow = noticesToShow.Where(n => n.cntCategory == selectedCategory).ToList();
                }

                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        CategoryComboBox.SelectionChanged -= CategoryComboBox_OnSelectionChanged;

                        BachecaViewModel.Categories = noticeCategories;

                        CategoryComboBox.SelectionChanged += CategoryComboBox_OnSelectionChanged;

                        //set the notices to show
                        BachecaViewModel.NoticesToShow = noticesToShow;
                    }
                );
            }
            finally
            {
                {
                    await CoreApplication.MainView.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal, () => { BachecaViewModel.IsLoadingBacheca = false; }
                    );
                }
            }
        }

        private void OnShouldUpdate(object sender, EventArgs args)
        {
            AggiornaAction();
        }

        private void ReadUnreadSegmented_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AggiornaAction();
        }

        public override async void AggiornaAction()
        {
            await Task.Run(async () => { await LoadData(); });
        }

        private void CategoryComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AggiornaAction();
        }
    }
}