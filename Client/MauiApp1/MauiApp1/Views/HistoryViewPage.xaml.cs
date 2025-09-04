using Microcharts.Maui; // ChartView�� ���� ���
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Microcharts;
using MauiApp1.Services;
using MauiApp1.Utils; // ChartEntry, LineChart ��
using MauiApp1.Data;
using MauiApp1.ModelViews;
using System.Text.Json;
namespace MauiApp1.Views;

public partial class HistoryViewPage : ContentPage
{
    public DiagnosisLogsViewModel _viewModel = new();  
    public HistoryViewPage()
    {
        InitializeComponent();
        
        BindingContext  =  _viewModel;
        //HttpService.Instance.ContextInit();
        _viewModel.SelectedIndexChangedAction = (index) =>
        {
            if (index >= 0)
            {
                DrawGraphByIndex(index);
            }
        };


    }
  private void OnPickerDateSelected(object sender, EventArgs e)
{
    int selectedIndex = DatePicker.SelectedIndex;
    if (selectedIndex >= 0)
    {
        DrawGraphByIndex(selectedIndex);
    }
}

    private async void DrawGraphByIndex(int index)
    {
        var log = _viewModel.GetDiagnosisLogIdx(index);
        Console.WriteLine("DEBUG: ���� ��� ����� ",log);
        if (log.diagnosis_result.HasValue)
        {
            var diagJson = log.diagnosis_result.Value;
            var classJson = diagJson.GetProperty("class").GetRawText();
            var regressionJson = diagJson.GetProperty("regression").GetRawText();

            var classData = JsonSerializer.Deserialize<DiagnosisClassification>(classJson);
            var regressionData = JsonSerializer.Deserialize<DiagnosisRegression>(regressionJson);

            // ������ �� Dictionary ��ȯ
            var classificationDict = new Dictionary<string, float>
            {
                { "�̸� �ָ�", classData.ForeheadWrinkle/15.0f },
                { "�̰� �ָ�", classData.FrownWrinkle/7.0f },
                { "���� �ָ�", classData.EyesWrinkle/7.0f },
                { "�� ���", classData.CheekPore/12.0f },
                { "�Լ� ����", classData.LipsDryness/5.0f },
                { "�� ó��", classData.JawSagging/7.0f }
            };

            var regressionDict = new Dictionary<string, float>
            {
                { "�� ��ü", regressionData.Face },
                { "�̸� ����", regressionData.ForeheadMoisture },
                { "�̸� ź��", regressionData.ForeheadElasticity },
                { "���� �ָ�", regressionData.EyesWrinkle },
                { "�� ����", regressionData.CheekMoisture },
                { "�� ź��", regressionData.CheekElasticity },
                { "�� ���", regressionData.CheekPore },
                { "�� ����", regressionData.JawMoisture },
                { "�� ź��", regressionData.JawElasticity }
            };

            await ChartUtil.SetRadarChartDataFloat(classChartView, classificationDict, "#68B9C0");
            await ChartUtil.SetRadarChartDataFloat(regrssionChartview, regressionDict, "#F37F64");
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDiagnosisLogAsync(); // ��¥ ����Ʈ ä���
                                                  // �ֽ� ���� �ڵ� ���
        if (HttpService.Instance.GetDiaLogEntries().Count > 0)
        {
            DrawGraphByIndex(HttpService.Instance.GetDiaLogEntries().Count - 1);
        }
        if (DiagnosisDataStore.Instance.IsUpdated)
        {
            DiagnosisResult? result = DiagnosisDataStore.Instance.LatestResult;
            if (result != null) {
                var classificationDict = new Dictionary<string, float>
                {{ "�̸� �ָ�", result.Classification?.ForeheadWrinkle/15.0f ?? 0  },
                { "�̰� �ָ�", result.Classification?.FrownWrinkle/7.0f ?? 0  },
                { "���� �ָ�", result.Classification?.EyesWrinkle/7.0f ?? 0  },
                { "�� ���", result.Classification?.CheekPore/12.0f ?? 0},
                { "�Լ� ����", result.Classification?.LipsDryness/5.0f ?? 0  },
                { "�� ó��", result.Classification?.JawSagging/7.0f  ??  0}};
                var regressionDict = new Dictionary<string, float>
                {{ "�� ��ü", result.Regression?.Face  ?? 0 },
                { "�̸� ����", result.Regression?.ForeheadMoisture  ?? 0},
                { "�̸� ź��", result.Regression?.ForeheadElasticity ?? 0 },
                { "���� �ָ�", result.Regression?.EyesWrinkle ?? 0  },
                { "�� ����", result.Regression?.CheekMoisture ?? 0 },
                { "�� ź��", result.Regression?.CheekElasticity ??0},
                { "�� ���", result.Regression?.CheekPore ?? 0  },
                { "�� ����", result.Regression?.JawMoisture ??  0},
                { "�� ź��", result.Regression?.JawElasticity ?? 0}
    };
                await ChartUtil.SetRadarChartDataFloat(classChartView, classificationDict, "#68B9C0");
                await ChartUtil.SetRadarChartDataFloat(regrssionChartview, regressionDict, "#F37F64");
            }
        }
    }
    //private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    //{
    //    var canvas = e.Surface.Canvas;
    //    canvas.Clear(SKColors.White);

    //    var center = new SKPoint(e.Info.Width / 2, e.Info.Height / 2);
    //    float radius = 100;

    //    int sides = 5;
    //    float angleStep = 360f / sides;
    //    var paint = new SKPaint
    //    {
    //        Color = SKColors.Black,
    //        StrokeWidth = 2,
    //        Style = SKPaintStyle.Stroke,
    //        IsAntialias = true
    //    };

    //    var path = new SKPath();
    //    for (int i = 0; i < sides; i++)
    //    {
    //        float angle = (float)(Math.PI * 2 * i / 5 - Math.PI / 2);
    //        var point = new SKPoint(
    //            center.X + radius * (float)Math.Cos(angle),
    //            center.Y + radius * (float)Math.Sin(angle));

    //        if (i == 0)
    //            path.MoveTo(point);
    //        else
    //            path.LineTo(point);
    //    }

    //    path.Close();
    //    canvas.DrawPath(path, paint);
    //}
    public void Draw_graph()
    {
        var classDataDict = new Dictionary<string, int>
    {
        { "�̸� �ָ�", 2 },
        { "�̰� �ָ�", 3 },
        { "���� �ָ�", 4 },
        { "�� ���", 3 },
        { "�Լ� ������", 2 },
        { "�� ó��", 4 }
    };

        var regressionData = new Dictionary<string, float>
    {
        { "�̸� ����", 72.5f },
        { "�̸� ź��", 64.2f },
        { "���� �ָ�", 55.3f },
        { "�� ����", 68.0f },
        { "�� ź��", 70.1f },
        { "�� ���", 50.5f },
        { "�� ����", 63.3f },
        { "�� ź��", 69.7f }
    };

        ChartUtil.SetRadarChartData(classChartView, classDataDict, "#68B9C0");
        ChartUtil.SetRadarChartDataFloat(regrssionChartview, regressionData, "#F37F64");
    }
    public void Draw_graph1()
    {
        classChartView.Chart = new RadarChart
        {
            Entries = new[]
    {
                new ChartEntry(80) { Label = "����", ValueLabel = "80", Color = SKColor.Parse("#266489") },
                new ChartEntry(60) { Label = "�ָ�", ValueLabel = "60", Color = SKColor.Parse("#68B9C0") },
                new ChartEntry(70) { Label = "ź��", ValueLabel = "70", Color = SKColor.Parse("#90D585") },
                new ChartEntry(50) { Label = "���", ValueLabel = "50", Color = SKColor.Parse("#F3C151") },
                new ChartEntry(90) { Label = "�Ǻ���", ValueLabel = "90", Color = SKColor.Parse("#F37F64") },
                new ChartEntry(65) { Label = "���", ValueLabel = "65", Color = SKColor.Parse("#424856") },
                new ChartEntry(85) { Label = "��Ƽ", ValueLabel = "85", Color = SKColor.Parse("#8F97A4") },
                new ChartEntry(75) { Label = "����", ValueLabel = "75", Color = SKColor.Parse("#DAC096") },
            }
        };
    }
}
