using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Utils
{
    public class FontUtil
    {
        /// <summary>
        /// microchart 는 내장된 폰트를 사용하는데 내장된 폰트를 사용하여
        /// 한글 출력시 에러
        /// </summary>
        /// <returns></returns>
        public static  async Task<SKTypeface> getNaunumFontAsync()
        {

            var font = SKTypeface.Default;

#if ANDROID

            using var stream = await FileSystem.OpenAppPackageFileAsync("NanumGothicCoding.ttf");
            font=  SKTypeface.FromStream(stream);
 
#elif WINDOWS
         var fontPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Fonts", "NanumGothicCoding.ttf");
            using var stream = File.OpenRead(fontPath);
            font=  SKTypeface.FromStream(stream);

#endif
            return font;
        }
    }
}
