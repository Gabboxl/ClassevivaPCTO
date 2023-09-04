using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace ClassevivaPCTO.Views
{
    public sealed partial class Valutazioni : Page
    {
        private Grades2Result _grades2Result;

        private readonly IClassevivaAPI apiWrapper;

        public Valutazioni()
        {
            this.InitializeComponent();

            App app = (App) App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            Card? cardResult = ViewModelHolder.GetViewModel().SingleCardResult;

            MainTitleTextBox.Text += VariousUtils.ToTitleCase(cardResult.firstName);


            await Task.Run(async () =>
            {
                _grades2Result = await apiWrapper.GetGrades(
                    cardResult.usrId.ToString()
                );

                var resultPeriods = await apiWrapper.GetPeriods(
                    cardResult.usrId.ToString()
                );


                //run on UI thread
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {

                    ProgressRingVoti.Visibility = Visibility.Collapsed;
                });
            });
        }


        


    }
}