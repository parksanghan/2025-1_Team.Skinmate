using CommunityToolkit.Maui.Views;
using MauiApp1.Services;

namespace MauiApp1.Views.Content;

public partial class WebMapPopup : Popup
{
    public WebMapPopup()
    {
        InitializeComponent();
        
     
        LoadLocationInit();
    
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
    private async void LoadLocationInit()
    {
            
        var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
        double lat = location?.Latitude ?? 37.5665;
        double lon = location?.Longitude ?? 126.9780;
        Console.WriteLine($"��ǥ ���: lat = {lat}, lon = {lon}");
        // ���� �Ķ���Ϳ� ��ǥ ���̱�
        MapWebView.Source = HttpService.Instance.RequestTMapLatLon(lat, lon);
//        MapWebView.Navigated += (s, e) =>
//        {
////#if ANDROID
////    if (MapWebView.Handler?.PlatformView is Android.Webkit.WebView androidWebView)
////    {

////        androidWebView.Settings.JavaScriptEnabled = true;
////        androidWebView.Settings.DomStorageEnabled = true;
////        androidWebView.Settings.SetSupportMultipleWindows(true);
////        androidWebView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
////    }
////#endif
//        };
    }
}