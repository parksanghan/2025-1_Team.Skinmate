using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Utils
{
    public class MediaUtil
    {
        public static async Task<bool> RequestGalleryPermissionAsync()
        {

            if (DeviceInfo.Version.Major >= 13)

            {
                var status = await Permissions.RequestAsync<Permissions.Media>();
                return status == PermissionStatus.Granted;
            }
            else
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                return status == PermissionStatus.Granted;
            }
            return true;
        }
        public static async Task<bool> CheckCameraPermission()
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            return status == PermissionStatus.Granted;
        }
        public static async Task<string> SaveCapturedImageAsync(byte[] imageBytes)
            {
                var path = string.Empty;
                try {
#if ANDROID
                //var dcimDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
                //var cameraDir = new Java.IO.File(dcimDir, "Camera");
                //if (!cameraDir.Exists()) cameraDir.Mkdir();
                //var filePath = System.IO.Path.Combine(cameraDir.AbsolutePath, $"captured_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
                //await File.WriteAllBytesAsync(filePath, imageBytes);
                //Console.WriteLine($"✅ 이미지 저장됨: {filePath}");
                // // ✅ 저장 후 MediaScanner 호출
                //Android.Content.Context context = Android.App.Application.Context;
                //Android.Content.Intent mediaScanIntent = new(Android.Content.Intent.ActionMediaScannerScanFile);
                //var contentUri = Android.Net.Uri.FromFile(new Java.IO.File(filePath));
                //mediaScanIntent.SetData(contentUri);
                //context.SendBroadcast(mediaScanIntent);
                //return path = filePath;

                return await SaveImageAndroidAsync(imageBytes);
#elif WINDOWS
                return await SaveImageWindowAsync(imageBytes);
#else
                    await Task.CompletedTask; // ⚠️ CS1998 제거용 dummy await
                    return path;
#endif

            }
                catch (Exception ex)
            {
                    Console.WriteLine($"❌ 예외 발생: {ex}"); return path;
            }
    }
        public static async Task<MultipartFormDataContent?> DataFromFilePickerAsync()
        {
            try
            {
                 

                var results = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "이미지 선택",
                    FileTypes = FilePickerFileType.Images
                });

                if (results == null || !results.Any())
                    return null;

                var form = new MultipartFormDataContent();
                int i = 0;
                foreach (var file in results)
                {
                    var stream = await file.OpenReadAsync();

                    // 바이트 크기 측정용 메모리 스트림 복사
                    using var memory = new MemoryStream();
                    await stream.CopyToAsync(memory);
                    var imageBytes = memory.ToArray();

                    var byteContent = new ByteArrayContent(imageBytes);
                    byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    // 디버깅 출력
                    Console.WriteLine($"{i}+[DEBUG] 파일 이름: {file.FileName}");
                    Console.WriteLine($"{i}[DEBUG] Content-Type: {byteContent.Headers.ContentType}");
                    Console.WriteLine($"{i}[DEBUG] Byte 크기: {imageBytes.Length}");

                    form.Add(byteContent, "files", file.FileName);
                    i++;
                }
                return form;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG]  DataFromFilePickerAsync 예외: {ex.Message}");
                return null;    
            }
        }
#if ANDROID
        private static async Task<string> SaveImageAndroidAsync(byte[] imageBytes)
        {
            var dcimDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim);
            var cameraDir = new Java.IO.File(dcimDir, "Camera");
            if (!cameraDir.Exists()) cameraDir.Mkdir();
            var filePath = System.IO.Path.Combine(cameraDir.AbsolutePath, $"captured_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
            await File.WriteAllBytesAsync(filePath, imageBytes);
            Console.WriteLine($"✅ 이미지 저장됨: {filePath}");
            // ✅ 저장 후 MediaScanner 호출
            Android.Content.Context context = Android.App.Application.Context;
            Android.Content.Intent mediaScanIntent = new(Android.Content.Intent.ActionMediaScannerScanFile);
            var contentUri = Android.Net.Uri.FromFile(new Java.IO.File(filePath));
            mediaScanIntent.SetData(contentUri);
            // 갱신알리는 용도 
            context.SendBroadcast(mediaScanIntent);
            return filePath;
        }
#endif


#if WINDOWS

        private async static Task<string> SaveImageWindowAsync(byte[] imageBytes )
        {
            try
            {
                // 바탕화면
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var filePath = Path.Combine(desktopPath, $"captured_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

                // 이미지 쓰기
                await File.WriteAllBytesAsync(filePath, imageBytes);

                Console.WriteLine($"✅ 이미지가 바탕화면에 저장되었습니다: {filePath}");

                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 이미지 저장 중 예외 발생: {ex.Message}");
                return string.Empty;
            }

        }
#endif
 
    }
}
