using CommunityToolkit.Maui.Views;

namespace MauiApp1.Views.Content;

public partial class CategoryPopup : Popup
{
   private readonly List<string> categories = new()
{
    "보습",         // 건조함 관리
    "미백",         // 피부톤 개선
    "주름 개선",     // 노화 방지
    "모공 축소",     // 넓은 모공 케어
    "여드름/트러블", // 민감성 or 지성 피부
    "자외선 차단",   // 선크림 관리
    "피부 진정",     // 붉음증, 열감 진정
    "각질 제거",     // 피지 및 각질 관리
    "탄력 강화",     // 리프팅/탄력
    "다크서클",      // 눈 밑 케어
};
    private readonly HashSet<string> selectedCategories = new(); // 중복 방지
    public CategoryPopup()
    {
        InitializeComponent();
        this.Color = Colors.Transparent;
        InitCategoryButtons();
    }
    public void InitCategoryButtons()
    {
        foreach (var category in categories)
        {
            var btn = new Button
            {
                Text = category,
                BackgroundColor = Colors.LightGray,
                Margin = new Thickness(5)
            };

            btn.Clicked += (s, e) =>
            {
                var b = (Button)s;
                if (selectedCategories.Contains(b.Text))
                {
                    selectedCategories.Remove(b.Text);
                    b.BackgroundColor = Colors.LightGray;
                }
                else
                {
                    selectedCategories.Add(b.Text);
                    b.BackgroundColor = Colors.LightGreen;
                }
            };

            CategoryContainer.Children.Add(btn);
        }
    }

    public void OnConfirmClicked(object sender, EventArgs e)
    {
        Close(selectedCategories.ToList());
    }
}