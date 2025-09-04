using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Google.MLKit.Vision.Face;
using Xamarin.Google.MLKit.Vision.Common;
using System.Linq;
using Android.Gms.Extensions;
using Android.Graphics;
namespace MauiApp1.Platforms.Android.AdAPi
{
    public class FaceDetectionService
    {
        private static FaceDetectionService? _instance;
        public static FaceDetectionService Instance=>_instance ??= new FaceDetectionService();  
        private IFaceDetector faceDetector;
        public FaceDetectionService()
        {
            //var opt = new  FaceDetectorOptions.Builder()
            //    .SetClassificationMode(FaceDetectorOptions.)
 
        }
    }
}
