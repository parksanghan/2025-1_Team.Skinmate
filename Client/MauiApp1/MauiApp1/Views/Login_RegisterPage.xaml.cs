using MauiApp1.Controller;
using MauiApp1.Model;
using MauiApp1.Services;

namespace MauiApp1.Views;

public partial class Login_RegisterPage : ContentPage
{
    private readonly AuthController authController;
        //<Entry x:Name="usernameEntry" Placeholder="아이디 입력" />
        //<Entry x:Name="passwordEntry" Placeholder="비밀번호 입력" IsPassword="True" />
	public Login_RegisterPage()
	{
		InitializeComponent();
        authController = new AuthController();  

    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            LoginRequest req = new LoginRequest
            {
                UserId = usernameEntry.Text,
                Password = passwordEntry.Text
            };
            string res = await HttpService.Instance.LoginAsync(req);
            if (res == "ok")
            {

                await DisplayAlert("로그인", "로그인을 성공하였습니다.", "확인");
                //Application.Current!.MainPage = new AppShell();
                // 권장 x 
                // 로그인 시 contextInit 작업과 메인페이지 작업 
                await HttpService.Instance.ContextInit();
                Application.Current!.Windows[0].Page = new AppShell();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("에러", ex.Message, "확인");
        }
        // 로그인 기능 구현
    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            RegisterRequest req = new RegisterRequest
            {
                UserId = usernameEntry.Text,
                Password = passwordEntry.Text,
                Name = usernameEntry.Text  
            };
            string res = await HttpService.Instance.RegisterAsync(req);
            Console.WriteLine($"[DEBUG] 서버 응답 내용: '{res}'");
        
            if (res == "ok") await DisplayAlert("가입", "가입을 성공하였습니다.", "확인");
        }
        catch (Exception ex)
        {
            await DisplayAlert("에러", ex.Message, "확인");
        }
        // 가입 기능 구현

    }
     

}