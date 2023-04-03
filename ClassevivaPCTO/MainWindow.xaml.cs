using ClassevivaPCTO.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;

namespace ClassevivaPCTO;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        //AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        //Content = null;

        AppWindowTitleBar titleBar = AppWindow.TitleBar;

        titleBar.ExtendsContentIntoTitleBar = true;
        titleBar.ButtonBackgroundColor = Colors.Transparent;
        titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;



        //SetTitleBar(AppTitleBar2);

        // AppWindow.Title

        //Title = "AppDisplayName".GetLocalized();



        this.Content = new Frame();

        var frame = (Frame)this.Content;

        frame.Navigate(typeof(Views.LoginPage), null);

        this.Activate(); //da rimvuone

    }
}
