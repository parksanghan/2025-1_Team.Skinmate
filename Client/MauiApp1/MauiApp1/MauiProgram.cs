using CommunityToolkit.Maui;
using MauiApp1.Controller;
using Microcharts.Maui; // ← 반드시 필요
#if ANDROID
using Android.Webkit; // ✅ 이거 추가!
using MauiApp1.Platforms.Android.AdHandler;
#endif
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.ConfigureMauiHandlers(handlers =>
            {

#if ANDROID
    Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("EnableJavaScript", (handler, view) =>
    {
        if (handler.PlatformView is Android.Webkit.WebView androidWebView)
        {
            androidWebView.Settings.JavaScriptEnabled = true;
            androidWebView.Settings.DomStorageEnabled = true;
            androidWebView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            androidWebView.Settings.CacheMode = CacheModes.Normal;
 
          
            androidWebView.Settings.SetSupportMultipleWindows(true);
            androidWebView.SetWebChromeClient(new Android.Webkit.WebChromeClient());

        }
    });
#endif
                ;
            })
                .UseMauiApp<Views.App>()
                .UseMauiCommunityToolkitCamera()
                .UseMicrocharts()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // 여기서 라이선스 등록
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF1cWWhPYVFwWmFZfVtgfV9DZlZVRmYuP1ZhSXxWdkBhXn9XdXJRQGJYV0B9XUs=");
            return builder.Build();
        }
    }
}
