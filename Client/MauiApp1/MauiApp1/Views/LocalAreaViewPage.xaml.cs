namespace MauiApp1.Views;

public partial class LocalAreaViewPage : ContentPage
{
	public LocalAreaViewPage()
	{
		InitializeComponent();
		myWebView.Source = "http://10.101.123.25:8080/web/main.html";


    }
}