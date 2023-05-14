using System;
using ClassevivaPCTO.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.DataModels
{
    public class OverviewDataModel : ObservableObject
    {
        public OverviewResult OverviewData { get; set; }

        public DateTime FilterDate { get; set; }

    }
}
