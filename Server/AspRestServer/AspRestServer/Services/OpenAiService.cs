// Services/OpenAiService.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AspRestServer.Models;

namespace AspRestServer.Services;

public class OpenAiService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public OpenAiService(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.openai.com/v1/");
        var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                 ?? cfg["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("OPENAI_API_KEY not set.");
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
    }

    // === 공개 API ===
    public async Task<string> RequestChatResponseAsync(IEnumerable<ChatLog> logs, string userMessage)
    {
        var chatList = BuildChatListFromLogs(logs);
        chatList.Insert(0, new() { ["role"] = "system", ["content"] = "You are a helpful assistant." });
        chatList.Add(new() { ["role"] = "user", ["content"] = userMessage });

        return await CallResponsesApiAsync(chatList);
    }

    public async Task<string> RequestChatDiagnosisAsync(IEnumerable<ChatLog> logs, object diagnosisResult)
    {
        var prompt = DiagnosisToQuestion(diagnosisResult);
        var chatList = BuildChatListFromLogs(logs);
        chatList.Insert(0, new() { ["role"] = "system", ["content"] = "You are a helpful assistant." });
        chatList.Add(new() { ["role"] = "user", ["content"] = prompt });

        return await CallResponsesApiAsync(chatList);
    }

    // === 내부 유틸 ===
    private static string DiagnosisToQuestion(object diagnosis)
    {
        string json = diagnosis is string s ? s : JsonSerializer.Serialize(diagnosis);
        return $"다음 피부 진단 결과를 사람에게 설명해줘:\n```json\n{json}\n```";
    }

    private static string SettingToQuestion(object setting)
    {
        string json = setting is string s ? s : JsonSerializer.Serialize(setting);
        return $"이게 내 관심사와 나이 성별인데 앞으로 이를 참고해서 대답해줘 :\n```json\n{json}\n```";
    }

    // EF ChatLog -> OpenAI messages
    private static List<Dictionary<string, string>> BuildChatListFromLogs(IEnumerable<ChatLog> logs)
    {
        var list = new List<Dictionary<string, string>>();

        foreach (var log in logs.OrderBy(l => l.Timestamp))
        {
            string? message = null;

            if (log.LogType == "진단분석")
            {
                var diag = string.IsNullOrWhiteSpace(log.DiagnosisResult) ? "{}" : log.DiagnosisResult;
                message = DiagnosisToQuestion(diag);
            }
            else if (log.LogType == "사용자설정")
            {
                var setting = string.IsNullOrWhiteSpace(log.Message) ? "{}" : log.Message;
                message = SettingToQuestion(setting);
            }
            else // "질의응답" 등 일반 채팅
            {
                message = log.Message;
            }

            if (!string.IsNullOrWhiteSpace(message))
                list.Add(new() { ["role"] = "user", ["content"] = message });

            if (!string.IsNullOrWhiteSpace(log.Response))
                list.Add(new() { ["role"] = "assistant", ["content"] = log.Response! });
        }

        return list;
    }

    private async Task<string> CallResponsesApiAsync(List<Dictionary<string, string>> chatList)
    {
        var payload = new { model = "gpt-4o-mini", input = chatList };
        var json = JsonSerializer.Serialize(payload, _json);
        using var res = await _http.PostAsync("responses", new StringContent(json, Encoding.UTF8, "application/json"));
        var body = await res.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("output_text", out var ot))
                return ot.GetString() ?? body;
        }
        catch { /* ignore */ }

        return body;
    }
}
