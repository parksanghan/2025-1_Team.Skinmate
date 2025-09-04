using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using MauiApp1.Utils;
using MauiApp1.Services;
using System.Text.Json;
using MauiApp1.Views.Content;
using MauiApp1.Data;
using MauiApp1.ModelViews;


namespace MauiApp1.Views;

public partial class CameraViewPage : ContentPage
{
    public CameraInfo? SelectedCamera { get; set; }
    // FilePickerData
    private MultipartFormDataContent? _selectedFileFormData;

    public CameraViewPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
    
    //  전면 카메라 변경
    private async void FrontBtnClicked(object sender, EventArgs e)
    {
        try
        {
#if ANDROID
            bool result = await MediaUtil.CheckCameraPermission();
            if (!result) return;
 
            var cameras = await cameraView.GetAvailableCameras(CancellationToken.None);
            if (cameras == null || cameras.Count == 0)
            {
                await DisplayAlert("경고", "사용 가능한 카메라가 없습니다", "확인");
                return;
            }

            SelectedCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Front);
            if (SelectedCamera == null)
            {
                await DisplayAlert("경고", "전면 카메라를 찾을 수 없습니다", "확인");
                return;
            }

            cameraView.SelectedCamera = SelectedCamera;
            await cameraView.StartCameraPreview(CancellationToken.None);
#endif
        }

        catch (Exception ex)
        {
            await DisplayAlert("에러", ex.Message, "확인");
        }

    }
    // 후면카메라 전환
    private async void RearBtnClicked(object sender, EventArgs e)
    {
        try
        {
#if ANDROID
            bool result = await MediaUtil.CheckCameraPermission();
            if (!result) return;
 
            var cameras = await cameraView.GetAvailableCameras(CancellationToken.None);

            if (cameras == null || cameras.Count == 0)
            {
                await DisplayAlert("경고", "사용 가능한 카메라가 없습니다", "확인");
                return;
            }

            SelectedCamera = cameras.FirstOrDefault(c => c.Position == CameraPosition.Rear);
            if (SelectedCamera == null)
            {
                await DisplayAlert("경고", "후면 카메라를 찾을 수 없습니다", "확인");
                return;
            }

            cameraView.SelectedCamera = SelectedCamera;
            await cameraView.StartCameraPreview(CancellationToken.None);
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlert("에러", ex.Message, "확인");
        }
         
    }
    // 진단 버튼
    private async void SendBtnClicked(object sender, EventArgs e)
    {
        try
        {
            // 진단버튼시 FilePicker로 가져온 데이터 MultipartFormDataContent
            if (SelectedCamera == null) return;
            else if (_selectedFileFormData != null)
            {
                var (classData, regressionData, result) = await HttpService.Instance.UploadFilesAsync(_selectedFileFormData);
                if (result == "ok") await DisplayAlert("업로드 성공", "업로드에 성공하였습니다.", "확인");
                //await DisplayAlert("클래스", JsonSerializer.Serialize(classData, new JsonSerializerOptions { WriteIndented = true }), "확인");
                //await DisplayAlert("회귀", JsonSerializer.Serialize(regressionData, new JsonSerializerOptions { WriteIndented = true }), "확인");
                DiagnosisDataStore.Instance.Update(classData,regressionData);
                string diagnosisResult = await HttpService.Instance.DiagnosisAsync(classData, regressionData);
                await ChatViewModel.Instance.AddBotMsg(diagnosisResult);
                string action = (string)await Application.Current.MainPage.ShowPopupAsync(new ResultPopup());

                switch (action)
                {
                    case "챗봇 이동":
                        await Shell.Current.GoToAsync("//ChatViewPage");
                        break;

                    case "결과 분석":
                        await Shell.Current.GoToAsync("//HistoryViewPage");
                        break;

                    case "취소":
                    case null:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("업로드 실패", ex.Message, "확인");
        }


    }
    // 캡처 버튼
    private async void CaptureBtnClicked(object sender, EventArgs e)
    {
        try
        {
            if (cameraView.SelectedCamera == null)
            {
                await DisplayAlert("경고", "카메라가 선택되지 않았습니다", "확인");
                return;
            }

            await Task.Delay(1000);
            await cameraView.CaptureImage(CancellationToken.None);
        }
        catch (Exception ex)
        {
            await DisplayAlert("예외 발생", $"메시지: {ex.Message}\n내부 예외: {ex.InnerException?.Message}", "확인");
        }

    }
    // 
    private async void CameraView_MediaCaptured(object sender, MediaCapturedEventArgs e)
    {
        Console.WriteLine("📸 MediaCaptured 이벤트 호출됨");
        try
        {
            using var memoryStream = new MemoryStream();
            await e.Media.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            Console.WriteLine($"[DEBUG] 이미지 바이트 크기: {imageBytes.Length}");
            var filePath = await MediaUtil.SaveCapturedImageAsync(imageBytes);
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("저장 완료", $"이미지가 저장되었습니다.\n{filePath}", "확인");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 예외 발생: {ex}");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("예외 발생", $"메시지: {ex.Message}\n스택: {ex.StackTrace}", "확인");
            });
        }

    }
    // 파일 가져오기 버튼
    private async void FileBtnClicked(object sender, EventArgs e)
    {
        try
        {
            if (!await MediaUtil.RequestGalleryPermissionAsync())
            {
                await DisplayAlert("권한 거부", "갤러리 접근 권한이 필요합니다.", "확인");
                return;
            }
            _selectedFileFormData =await MediaUtil.DataFromFilePickerAsync();
            if (_selectedFileFormData == null) await DisplayAlert("오류", "파일을 선택하세요.", "확인");
           
        }
        catch (Exception ex)
        {
            await DisplayAlert("예외", ex.Message, "확인");
}
    }
 
}