using MauiApp1.Controller;
using MauiApp1.Model;
using MauiApp1.Services;

namespace MauiApp1.Views;

public partial class Login_RegisterPage : ContentPage
{
    private readonly AuthController authController;
        //<Entry x:Name="usernameEntry" Placeholder="���̵� �Է�" />
        //<Entry x:Name="passwordEntry" Placeholder="��й�ȣ �Է�" IsPassword="True" />
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

                await DisplayAlert("�α���", "�α����� �����Ͽ����ϴ�.", "Ȯ��");
                //Application.Current!.MainPage = new AppShell();
                // ���� x 
                // �α��� �� contextInit �۾��� ���������� �۾� 
                await HttpService.Instance.ContextInit();
                Application.Current!.Windows[0].Page = new AppShell();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("����", ex.Message, "Ȯ��");
        }
        // �α��� ��� ����
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
            Console.WriteLine($"[DEBUG] ���� ���� ����: '{res}'");
        
            if (res == "ok") await DisplayAlert("����", "������ �����Ͽ����ϴ�.", "Ȯ��");
        }
        catch (Exception ex)
        {
            await DisplayAlert("����", ex.Message, "Ȯ��");
        }
        // ���� ��� ����

    }
     

}