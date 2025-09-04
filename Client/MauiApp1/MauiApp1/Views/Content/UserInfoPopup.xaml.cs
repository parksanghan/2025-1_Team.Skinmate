using CommunityToolkit.Maui.Views;

namespace MauiApp1.Views.Content;

public partial class UserInfoPopup : Popup
{
    private string selectedGender = string.Empty;
    private string selectedAgeRange = string.Empty;
    public UserInfoPopup()
	{
		InitializeComponent();
        InitGenderButtons();
        InitAgePicker();
    }
    public void InitGenderButtons()
    {
        var genders = new[] { "남성", "여성" };

        foreach (var gender in genders)
        {
            var btn = new Button
            {
                Text = gender,
                BackgroundColor = Colors.LightGray,
                CornerRadius = 12
            };

            btn.Clicked += (s, e) =>
            {
                selectedGender = gender;

                // 모든 버튼 초기화
                foreach (var view in GenderContainer.Children)
                    ((Button)view).BackgroundColor = Colors.LightGray;

                // 선택한 버튼 강조
                ((Button)s).BackgroundColor = Colors.LightGreen;
            };

            GenderContainer.Children.Add(btn);
        }
    }
    public void InitAgePicker()
    {
        var ages = new[] { "10대", "20대", "30대", "40대", "50대","60대 이상" };

        foreach (var age in ages)
        {
            AgePicker.Items.Add(age);
        }

        AgePicker.SelectedIndexChanged += (s, e) =>
        {
            selectedAgeRange = AgePicker.SelectedItem.ToString();
        };
    }
    public void OnConfirmClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(selectedGender) && !string.IsNullOrEmpty(selectedAgeRange))
        {
            var result = new { Gender = selectedGender, Age = selectedAgeRange };
            Close(result);
        }
        else
        {
            // 필수 항목 선택 안내
            Application.Current.MainPage.DisplayAlert("입력 누락", "성별과 나이대를 선택해주세요.", "확인");
        }
    }
}