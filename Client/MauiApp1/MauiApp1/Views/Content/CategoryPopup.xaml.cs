using CommunityToolkit.Maui.Views;

namespace MauiApp1.Views.Content;

public partial class CategoryPopup : Popup
{
   private readonly List<string> categories = new()
{
    "����",         // ������ ����
    "�̹�",         // �Ǻ��� ����
    "�ָ� ����",     // ��ȭ ����
    "��� ���",     // ���� ��� �ɾ�
    "���帧/Ʈ����", // �ΰ��� or ���� �Ǻ�
    "�ڿܼ� ����",   // ��ũ�� ����
    "�Ǻ� ����",     // ������, ���� ����
    "���� ����",     // ���� �� ���� ����
    "ź�� ��ȭ",     // ������/ź��
    "��ũ��Ŭ",      // �� �� �ɾ�
};
    private readonly HashSet<string> selectedCategories = new(); // �ߺ� ����
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