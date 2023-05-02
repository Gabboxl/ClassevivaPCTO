using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ClassevivaPCTO.ViewModels
{
    public class OverviewDataModel : ObservableObject
    {
        public OverviewResult OverviewData { get; set; }

        public DateTime FilterDate { get; set; }

        public OverviewDataModel(OverviewResult overviewData, DateTime filterDate)
        {
            OverviewData = overviewData;
            FilterDate = filterDate;
        }
    }
}
