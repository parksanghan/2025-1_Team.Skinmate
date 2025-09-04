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
        var genders = new[] { "����", "����" };

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

                // ��� ��ư �ʱ�ȭ
                foreach (var view in GenderContainer.Children)
                    ((Button)view).BackgroundColor = Colors.LightGray;

                // ������ ��ư ����
                ((Button)s).BackgroundColor = Colors.LightGreen;
            };

            GenderContainer.Children.Add(btn);
        }
    }
    public void InitAgePicker()
    {
        var ages = new[] { "10��", "20��", "30��", "40��", "50��","60�� �̻�" };

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
            // �ʼ� �׸� ���� �ȳ�
            Application.Current.MainPage.DisplayAlert("�Է� ����", "������ ���̴븦 �������ּ���.", "Ȯ��");
        }
    }
}