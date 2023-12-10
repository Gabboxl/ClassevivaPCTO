using ClassevivaPCTO.Adapters;
using ClassevivaPCTO.Helpers.Palettes;
using ClassevivaPCTO.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace ClassevivaPCTO.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _version;

        [ObservableProperty]
        private List<ComboPaletteAdapter> _comboPalettes;

        [ObservableProperty]
        private PaletteType _paletteType = PaletteSelectorService.PaletteEnum;

        [ObservableProperty]
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
    }
}
