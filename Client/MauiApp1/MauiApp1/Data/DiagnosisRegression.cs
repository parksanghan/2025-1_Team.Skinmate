using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiApp1.Data
{
    public class DiagnosisRegression
    {
        //얼굴 전체
        [JsonPropertyName("face")]
        public float Face { get; set; }
        //이마 수분
        [JsonPropertyName("forehead_moisture")]
        public float ForeheadMoisture { get; set; }
        //이마 탄력
        [JsonPropertyName("forehead_elasticity")]
        public float ForeheadElasticity { get; set; }
        //눈가 주름
        [JsonPropertyName("eyes_wrinkle")]
        public float EyesWrinkle { get; set; }
        //볼 수분
        [JsonPropertyName("cheek_moisture")]
        public float CheekMoisture { get; set; }
        // 볼 탄력
        [JsonPropertyName("cheek_elasticity")]
        public float CheekElasticity { get; set; }
        //볼 모공
        [JsonPropertyName("cheek_pore")]
        public float CheekPore { get; set; }
        //턱 수분
        [JsonPropertyName("jaw_moisture")]
        public float JawMoisture { get; set; }
        //턱 탄력
        [JsonPropertyName("jaw_elasticity")]
        public float JawElasticity { get; set; }
    }
}
