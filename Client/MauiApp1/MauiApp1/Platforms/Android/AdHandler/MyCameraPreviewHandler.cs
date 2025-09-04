using Android.Content;
using MauiApp1.Controller;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Platforms.Android.AdHandler
{
    // 여기서매핑해줌
    public class MyCameraPreviewHandler : ViewHandler<MyCameraPreview, Camera2Preview>
    {
        public MyCameraPreviewHandler() : base(Mapper)
        {
             
        }
 
        public static IPropertyMapper<MyCameraPreview, MyCameraPreviewHandler> Mapper =
        new PropertyMapper<MyCameraPreview, MyCameraPreviewHandler>(ViewHandler.ViewMapper);
        protected override Camera2Preview CreatePlatformView()
        {
            return new Camera2Preview(Context);
        }
    }
}
