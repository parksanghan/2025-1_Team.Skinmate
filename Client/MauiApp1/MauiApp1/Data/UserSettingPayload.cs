using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiApp1.Data
{
    public class UserSettingPayload
    {
        [JsonPropertyName("interests")]
        public List<string> Interests { get; set; } = new();

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;

        [JsonPropertyName("age")]
        public string Age { get; set; } = string.Empty;
    }
}
