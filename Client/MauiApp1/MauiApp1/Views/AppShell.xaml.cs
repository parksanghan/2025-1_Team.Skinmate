namespace MauiApp1.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(ChatViewPage), typeof(ChatViewPage));
        Routing.RegisterRoute(nameof(CameraViewPage), typeof(CameraViewPage));
        Routing.RegisterRoute(nameof(HistoryViewPage), typeof(HistoryViewPage));
    }
}
