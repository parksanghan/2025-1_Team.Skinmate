using CommunityToolkit.Maui.Views;

namespace MauiApp1.Views.Content;

public partial class ResultPopup : Popup
{
    public ResultPopup()
    {
        InitializeComponent();
    }

    private void OnChatbotClicked(object sender, EventArgs e)
    {
        Close("Ãªº¿ ÀÌµ¿");
    }

    private void OnAnalyzeClicked(object sender, EventArgs e)
    {
        Close("°á°ú ºÐ¼®");
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close("Ãë¼Ò");
    }
}
