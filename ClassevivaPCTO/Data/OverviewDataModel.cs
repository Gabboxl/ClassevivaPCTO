using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Windows.UI.Xaml.Data;

namespace ClassevivaPCTO.ViewModels
{
    public class OverviewDataModel : ObservableObject
    {
        public OverviewResult OverviewData { get; set; }

        public DateTime FilterDate { get; set; }

    }
}
