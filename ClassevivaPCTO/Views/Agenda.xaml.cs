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
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace ClassevivaPCTO.Views
{
    public sealed partial class Agenda : Page
    {
        public AgendaViewModel AgendaViewModel { get; } = new AgendaViewModel();

        private readonly IClassevivaAPI apiWrapper;

        public Agenda()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AgendaViewModel.IsLoadingAgenda = true;

            //listender for calendaragenda date change
            CalendarAgenda.DateChanged += CalendarAgenda_DateChanged;

            //imposto la data di oggi del picker, e aziono il listener per il cambiamento della data
            CalendarAgenda.Date = DateTime.Now;
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
                if (CalendarAgenda.Date.Value.Date == DateTime.Now.Date)
                {
                    ButtonToday.IsChecked = true;
                    //ButtonToday.IsEnabled = false;
                }
                else
                {
                    ButtonToday.IsChecked = false;
                    //ButtonToday.IsEnabled = true;
                }

                await Task.Run(async () => { await LoadData(agendaSelectedDate.Value.Date); });
            }
        }

        private async Task LoadData(DateTime dateToLoad)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () => { AgendaViewModel.IsLoadingAgenda = true; }
            );

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            string apiDate = VariousUtils.ToApiDateTime(dateToLoad);

            OverviewResult overviewResult = await apiWrapper.GetOverview(
                cardResult.usrId.ToString(),
                apiDate,
                apiDate,
                loginResult.token
            );

            //update UI on UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    //create new OverviewDataModel instance and set the data var inside
                    var overviewData = new OverviewDataModel
                    {
                        OverviewData = overviewResult,
                        FilterDate = dateToLoad
                    };

                    OverviewListView.ItemsSource = overviewData;

                    AgendaViewModel.IsLoadingAgenda = false;
                }
            );
        }

        private void ButtonToday_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CalendarAgenda.Date = DateTime.Now;
        }

        private void ButtonYesterday_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CalendarAgenda.Date = CalendarAgenda.Date.Value.AddDays(-1);
        }

        private void ButtonTomorrow_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //add one day to the calendaragenda date
            CalendarAgenda.Date = CalendarAgenda.Date.Value.AddDays(1);
        }

        private async void AggiornaCommand_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var agendaSelectedDate = CalendarAgenda.Date;

            await Task.Run(async () => { await LoadData(agendaSelectedDate.Value.Date); });
        }


        private void PopupAgendaButton_OnClick(object sender, RoutedEventArgs e)
        {
            //this.StatusPane.IsPaneOpen = !this.StatusPane.IsPaneOpen;
            AgendaPopup.Height = this.ActualHeight;
            AgendaPopup.IsOpen = true;
        }

        private async void PopupLessonsButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () => { await LoadLessonsPopup(); });
        }


        private async Task LoadLessonsPopup()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    LezioniPopup.Height =
                        this.ActualHeight; //set the height of the popup to the height of the current PAGE (not the window because we do not need to take into account the appbar space)

                    LezioniPopupStackPanel.Children.Clear();
                });

            LoginResultComplete loginResult = ViewModelHolder.getViewModel().LoginResult;
            Card cardResult = ViewModelHolder.getViewModel().SingleCardResult;

            SubjectsResult subjects = await apiWrapper.GetSubjects(
                cardResult.usrId.ToString(),
                loginResult.token
            );

            //var StartDate if i am on the first semester, then start date is 1st of september of the current year
            //else start date is 1st of september of the next year
            DateTime startDate = new DateTime(
                DateTime.Now.Month >= 9 ? DateTime.Now.Year : DateTime.Now.Year - 1,
                9,
                1
            );


            //var EndDate of next year + june 30th
            DateTime endDate = new DateTime(
                DateTime.Now.Month <= 7 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                6, 30);


            LessonsResult lessons = await apiWrapper.GetLessons(
                cardResult.usrId.ToString(),
                VariousUtils.ToApiDateTime(startDate),
                VariousUtils.ToApiDateTime(endDate),
                loginResult.token
            );

            //add an expander control for 10 times in a loop to the LezioniPopupStackPanel
            foreach (var currentSubject in subjects.Subjects)
            {
                //list of lessons for the current subject id
                var subjectLessons = lessons.Lessons.Where(lesson => lesson.subjectId == currentSubject.id).ToList();
                await CoreApplication.MainView.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        var expander = new Expander
                        {
                            Header = currentSubject.description,
                            //Content = "yoyo" 
                        };


                        var listviewlessons = new LessonsListView()
                        {
                            ItemsSource = subjectLessons,
                        };

                        expander.Content = listviewlessons;

                        LezioniPopupStackPanel.Children.Add(expander);
                    });
            }


            await CoreApplication.MainView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    //we open the popup
                    LezioniPopup.IsOpen = true;
                });
        }
    }
}