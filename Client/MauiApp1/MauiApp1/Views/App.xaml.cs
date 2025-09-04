using MauiApp1.Services;

namespace MauiApp1.Views;

public partial class App : Application
{
    public App()
    {
        InitializeComponent(); 
        //  앱 시작시 context 초기화
         
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new Login_RegisterPage());    
        // 처음시작시에는 로그인 페이지만 진입
        // 이후 로그인 성공 시 contextinit과 appshell 진입
    }
}