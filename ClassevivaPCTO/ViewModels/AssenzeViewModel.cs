﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassevivaPCTO.ViewModels
{
    public partial class AssenzeViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoadingAssenze = true;

        [ObservableProperty]
        private bool _showShimmers = true;
    }
}