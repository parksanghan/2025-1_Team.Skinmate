using Android.App;
using Android.Content.PM;
using Android.OS;
using Android;

namespace MauiApp1
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            if (CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted)
            {
                RequestPermissions(new[] { Manifest.Permission.Camera }, 0);
            }

            // ✅ 상태바 색상 적용 코드
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#E1EAF2"));
            }
        }
    }
}