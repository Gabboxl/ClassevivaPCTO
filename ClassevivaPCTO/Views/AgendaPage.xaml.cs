﻿using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ClassevivaPCTO.Controls;
using ClassevivaPCTO.DataModels;
using Expander = Microsoft.UI.Xaml.Controls.Expander;

namespace ClassevivaPCTO.Views
{
    public sealed partial class AgendaPage : CustomAppPage
    {
        public AgendaViewModel AgendaViewModel { get; } = new();

        private readonly IClassevivaAPI _apiWrapper;

        private SubjectsResult _subjects;
        private LessonsResult _lessons;

        public AgendaPage()
        {
            InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            _apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AgendaViewModel.IsLoadingAgenda = true;

            //listender for calendaragenda date change
            CalendarAgenda.DateChanged += CalendarAgenda_DateChanged;

            //imposto la data di oggi del picker, e aziono il listener per il cambiamento della data
            CalendarAgenda.Date = DateTime.Now;


            //set the min and max date of the calendaragenda
            var agedaDates = VariousUtils.GetAgendaStartEndDates();

            CalendarAgenda.MinDate = agedaDates.startDate;
            CalendarAgenda.MaxDate = agedaDates.endDate;
        }

        private async void CalendarAgenda_DateChanged(
            CalendarDatePicker sender,
            CalendarDatePickerDateChangedEventArgs args
        )
        {
            var agendaSelectedDate = CalendarAgenda.Date;

            if (agendaSelectedDate.HasValue)
            {
                //if the date is today, then the button to go to today is disabled
                if (agendaSelectedDate.Value.Date == DateTime.Now.Date)
                {
                    ButtonToday.IsChecked = false;
                    //ButtonToday.IsEnabled = false;
                }
                else
                {
                    ButtonToday.IsChecked = true;
                    //ButtonToday.IsEnabled = true;
                }

                await Task.Run(async () => { await LoadData(agendaSelectedDate.Value.Date); });
            }
        }

        private async Task LoadData(DateTime dateToLoad)
        {
            try
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () => { AgendaViewModel.IsLoadingAgenda = true; }
                );

                Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

                string apiDate = VariousUtils.ToApiDateTime(dateToLoad);

                OverviewResult overviewResult = await _apiWrapper.GetOverview(
                    cardResult.usrId.ToString(),
                    apiDate,
                    apiDate
                );

                //update UI on UI thread
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        //create new OverviewDataModel instance and set the data var inside
                        var overviewData = new OverviewDataModel
                        {
                            OverviewData = overviewResult,
                            FilterDate = dateToLoad
                        };

                        OverviewListView.ItemsSource = overviewData;
                    }
                );
            }
            finally
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () => { AgendaViewModel.IsLoadingAgenda = false; }
                );
            }
        }

        private void ButtonToday_Click(object sender, RoutedEventArgs e)
        {
            CalendarAgenda.Date = DateTime.Now;
        }

        private void ButtonYesterday_Click(object sender, RoutedEventArgs e)
        {
            CalendarAgenda.Date = CalendarAgenda.Date.Value.AddDays(-1);
        }

        private void ButtonTomorrow_Click(object sender, RoutedEventArgs e)
        {
            //add one day to the calendaragenda date
            CalendarAgenda.Date = CalendarAgenda.Date.Value.AddDays(1);
        }

        public override async void AggiornaAction()
        {
            var agendaSelectedDate = CalendarAgenda.Date;

            await Task.Run(async () => { await LoadData(agendaSelectedDate.Value.Date); });
        }

        private async void PopupAgendaButton_OnClick(object sender, RoutedEventArgs e)
        {
            //this.StatusPane.IsPaneOpen = !this.StatusPane.IsPaneOpen;
            await Task.Run(async () => { await LoadAgendaPopup(); });
        }

        private async void PopupLessonsButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () => { await LoadLessonsPopup(); });
        }

        private async Task LoadLessonsPopup()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    LezioniPopup.Height = ActualHeight; //set the height of the popup to the height of the current PAGE (not the window because we do not need to take into account the appbar space)

                    LezioniPopupStackPanel.Children.Clear();

                    LezioniPopupProgressRing.IsActive = true;

                    LezioniPopup.IsOpen = true;
                });

            LoginResultComplete? loginResult = AppViewModelHolder.GetViewModel().LoginResult;
            Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;


            if (_subjects == null)
            {
                _subjects = await _apiWrapper.GetSubjects(
                    cardResult.usrId.ToString()
                );
            }

            if (_lessons == null)
            {
                var dates = VariousUtils.GetLessonsStartEndDates();

                _lessons = await _apiWrapper.GetLessons(
                    cardResult.usrId.ToString(),
                    VariousUtils.ToApiDateTime(dates.startDate),
                    VariousUtils.ToApiDateTime(dates.endDate)
                );
            }

            //add an expander control for 10 times in a loop to the LezioniPopupStackPanel
            foreach (var currentSubject in _subjects.Subjects)
            {
                //list of lessons for the current subject id
                var subjectLessons = _lessons.Lessons.Where(lesson => lesson.subjectId == currentSubject.id).ToList();
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        var expander = new Expander
                        {
                            Header = currentSubject.description,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            HorizontalContentAlignment = HorizontalAlignment.Stretch
                            //Content = "yoyo" 
                        };


                        var listviewlessons = new LessonsListView
                        {
                            EnableEmptyAlert = true,
                            IsSingleSubjectList = true,
                            ItemsSource = subjectLessons,
                            HorizontalAlignment = HorizontalAlignment.Stretch
                        };

                        expander.Content = listviewlessons;

                        LezioniPopupStackPanel.Children.Add(expander);
                    });
            }

            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    //we open the popup
                    LezioniPopup.IsOpen = true;

                    LezioniPopupProgressRing.IsActive = false;
                });
        }

        private async Task LoadAgendaPopup()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    AgendaPopup.Height = ActualHeight; //set the height of the popup to the height of the current PAGE (not the window because we do not need to take into account the appbar space)

                    AgendaPopupListviewContainer.Children.Clear();

                    AgendaPopupProgressRing.IsActive = true;

                    AgendaPopup.IsOpen = true;
                });

            LoginResultComplete? loginResult = AppViewModelHolder.GetViewModel().LoginResult;
            Card? cardResult = AppViewModelHolder.GetViewModel().SingleCardResult;

            var dates = VariousUtils.GetAgendaStartEndDates();

            AgendaResult agendaEvents = await _apiWrapper.GetAgendaEvents(
                cardResult.usrId.ToString(),
                VariousUtils.ToApiDateTime(dates.startDate),
                VariousUtils.ToApiDateTime(dates.endDate)
            );

            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    var agendaListView = new AgendaMultipleDaysListView
                    {
                        ItemsSource = agendaEvents.AgendaEvents,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    AgendaPopupListviewContainer.Children.Add(agendaListView);
                });

            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    //we open the popup
                    AgendaPopup.IsOpen = true;

                    AgendaPopupProgressRing.IsActive = false;
                });
        }
    }
}