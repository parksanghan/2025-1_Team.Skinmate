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
        Close("ê�� �̵�");
    }

    private void OnAnalyzeClicked(object sender, EventArgs e)
    {
        Close("��� �м�");
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close("���");
    }
}
