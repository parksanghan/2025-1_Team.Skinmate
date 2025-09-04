using CommunityToolkit.Maui.Views;
using MauiApp1.Data;
using MauiApp1.ModelViews;
using MauiApp1.Services;
using MauiApp1.Views.Content;
using System.Reflection;
using System.Text.Json;

namespace MauiApp1.Views;

public partial class ChatViewPage : ContentPage
{
    private readonly ChatViewModel viewModel;
  
    public ChatViewPage()
	{
		InitializeComponent();
        // new 에서 ChatViewModel.Instance로 변경 
        viewModel = ChatViewModel.Instance;
        BindingContext = viewModel;
    }
    private void ScrollToBottom()
    {
        if (chatCollectionView.ItemsSource is IList<ChatMessage> items && items.Count > 0)
        {
            Dispatcher.Dispatch(() =>
            {
                chatCollectionView.ScrollTo(items.Count - 1, position: ScrollToPosition.End, animate: false);
            });
        }
    }
    private async void OnSendClicked(object sender, EventArgs e)
    {
        string message = chatEntry.Text;

        // 아래부분은 
        var shakeAnim = new Animation();
        shakeAnim.WithConcurrent(new Animation(v => chatBtn.TranslationX = v, 0, -15), 0, 0.1);
        shakeAnim.WithConcurrent(new Animation(v => chatBtn.TranslationX = v, -15, 15), 0.1, 0.3);
        shakeAnim.WithConcurrent(new Animation(v => chatBtn.TranslationX = v, 15, -15), 0.3, 0.5);
        shakeAnim.WithConcurrent(new Animation(v => chatBtn.TranslationX = v, -15, 0), 0.5, 0.6);
        chatBtn.Animate("shake", shakeAnim, 16, 250);
#if ANDROID
        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
#endif
        await viewModel.AddUserMsg(message);
        ScrollToBottom();
        chatEntry.Text = "";  // 입력창 비우기
        string res = await HttpService.Instance.SendChatMessageAsync(message);
        await viewModel.AddBotMsg(res);
        ScrollToBottom();
        //if (!string.IsNullOrWhiteSpace(message))
        //{
        //    await viewModel.simulation(message);  // ✨ 여기서 호출!
        //    chatEntry.Text = "";  // 입력창 비우기
        //}
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (HttpService.Instance._userSettingPayload == null)
        {
    

            var result = await this.ShowPopupAsync(new CategoryPopup());
            var rr = await this.ShowPopupAsync(new UserInfoPopup());

            //if (result is List<string> list1 && rr is List<string> list2)
            //{
            //    var combined = list1.Concat(list2).ToList();
            //    await DisplayAlert("선택 완료", $"선택된 항목: {string.Join(", ", combined)}", "확인");
            //}
            var gender = rr.GetType().GetProperty("Gender")?.GetValue(rr)?.ToString();
            var age = rr.GetType().GetProperty("Age")?.GetValue(rr)?.ToString();

            if (result is List<string> list1 && rr is { } userInfo)
            {

                await DisplayAlert("사용자 정보", $"성별: {gender}, 나이대: {age}", "확인");
                await DisplayAlert("선택 완료", $"선택된 항목: {string.Join(", ", list1)}", "확인");
                var payload = new UserSettingPayload
                {
                    Interests = list1,
                    Gender = gender ?? "",
                    Age = age ?? ""
                };
                string message =  await HttpService.Instance.SendUserSettingAsync(payload); 
                await viewModel.AddBotMsg(message);
                HttpService.Instance._userSettingPayload = payload;
            }
             
          
         
        }
    }

}