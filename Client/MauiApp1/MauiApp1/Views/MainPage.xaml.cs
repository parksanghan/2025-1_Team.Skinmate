namespace MauiApp1.Views;
using CommunityToolkit.Maui.Camera;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using MauiApp1.Views.Content;
using System.Threading.Tasks;

public partial class MainPage : ContentPage
{
    
    
    public MainPage()
    {
        InitializeComponent();
    
    }

 
    private async void OnLocationClick(object sender, EventArgs e)
    {
        var popup = new WebMapPopup();
        await this.ShowPopupAsync(popup);

    }

 
    private async void OnDiagnosisClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CameraViewPage");
    }

    private async void OnChatClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ChatViewPage");
    }

    private async void OnLogClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HistoryViewPage");
    }
}
