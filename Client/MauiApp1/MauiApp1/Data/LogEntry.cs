using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp1.Data
{
    public class LogEntry
    {
        public int chat_id { get; set; }
        public int user_id { get; set; }
        public string log_type { get; set; }
        public string? image_path { get; set; }
        public JsonElement? diagnosis_result { get; set; }
        public string? message { get; set; }
        public string? response { get; set; }
        public DateTime timestamp { get; set; }
    }
}
