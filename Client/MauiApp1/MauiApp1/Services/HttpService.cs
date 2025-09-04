
 
using MauiApp1.Data;
using MauiApp1.Model;
using MauiApp1.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    public class HttpService
    {
        private static HttpService? _instance;
        private static readonly object _lock = new();
        private readonly HttpClient _httpClient;
        private const string Ip = "172.30.1.100";//172.301.1.98 10.101.41.233"
        private const string BaseUrl = $"http://skinmate.shop";
        private const string BaseUrl1 = $"http://{Ip}:8080";
        private string? MyId { get; set; }
        private List< LogEntry?> _diaLogEntry; // 진단결과 로그
        private List<LogEntry?> _chatLgoEntry;// 채팅 로그 
        private LogEntry _userLogSetting;
        public UserSettingPayload? _userSettingPayload  =null; //유저 설정 로그

        public IReadOnlyList<LogEntry?> GetDiaLogEntries() => _diaLogEntry;
        public static HttpService Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new HttpService();
                }
            }
        }
        private HttpService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var url = $"{BaseUrl}/login"; // 실제 서버 IP로 변경
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync(url, content);
            var jsonRes = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] 응답 본문: {jsonRes}");
            if (res.IsSuccessStatusCode)
            {

                var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes);
                string result = obj["status"].ToLower();
                Console.WriteLine($"[DEBUG] 로그인 성공: {result}");
                this.MyId = request.UserId; // 여기서 자체 id 할당
            
                return result;
            }
            else
            {
                throw new Exception($"서버 오류: {jsonRes}");
            }
        }
        public async Task<String> RegisterAsync(RegisterRequest request)
        {

            var url = $"{BaseUrl}/register"; // 실제 서버 IP로 변경
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync(url, content);
            var jsonRes = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] 응답 본문: {jsonRes}");

            if (res.IsSuccessStatusCode)
            {
                var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes);
                string result = obj["status"].ToLower();
                Console.WriteLine($"[DEBUG] 가입 성공: {result}");
                return result;
            }
            else
            {
                throw new Exception($"서버 오류: {jsonRes}");
            }

        }
        public async Task<(DiagnosisClassification?, DiagnosisRegression?, string? reuslt)> UploadFilesAsync(MultipartFormDataContent data)
        {
            try
            {
                var url = $"{BaseUrl}/{this.MyId}/upload";
                var res = await _httpClient.PostAsync(url, data);
                var jsonRes = await res.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] 응답 본문: {jsonRes}");
                if (res.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(jsonRes);
                    var root = doc.RootElement;
                    string result = root.GetProperty("status").GetString().ToLower();
                    Console.WriteLine($"[DEBUG] 업로드 성공: {result}");
             
                    // 분류모델 진단결과
                    var diagnosisResult = root.GetProperty("diagnosis_result");
                    // 회귀모델 진단결과
                    var classJson = diagnosisResult.GetProperty("class").GetRawText();
                    var regressionJson = diagnosisResult.GetProperty("regression").GetRawText();
                    var classData = JsonSerializer.Deserialize<DiagnosisClassification>(classJson);
                    var regressionData = JsonSerializer.Deserialize<DiagnosisRegression>(regressionJson);
                    // 분류 모델결과 , 회귀모델결과 반환 후 이후에 확인누르면 history 채널로가는데 이때 이 
                    // 2개인자를 넘김으로 또 그래프 그릴예정
                    return (classData,regressionData, result);
                }
                else
                {
                    throw new Exception($"업로드 실패: {await res.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] UploadFilesAsync 예외: {ex.Message}");
                throw;// 그대로 던짐
            }

        }
        public async Task<string> SendChatMessageAsync(string message)
        {
            var url = $"{BaseUrl}/{this.MyId}/chat";
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("message", message)
        });

            var res = await _httpClient.PostAsync(url, content);
            var jsonRes = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] 응답 본문: {jsonRes}");

            if (res.IsSuccessStatusCode)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRes, options);
                    if (obj != null && obj.TryGetValue("status", out var status) && status == "ok")
                    {
                        return obj.TryGetValue("msg", out var msg) ? msg : "";
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] JSON 파싱 오류: {ex.Message}");
                    return "";
                }
            }
            else
            {
                throw new Exception($"채팅 실패: {jsonRes}");
            }
        }
        public async Task<List<LogEntry>> GetLogMessageAsync()
        {
            try
            {
                /*var url = $"{BaseUrl}/{this.MyId}/logs";*/
                var url = $"{BaseUrl}/{this.MyId}/logs";
                var res = await _httpClient.GetAsync(url);
                var jsonRes = await res.Content.ReadAsStringAsync();
                if (res.IsSuccessStatusCode)
                {
                    var opt = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var logs =  JsonSerializer.Deserialize<List<LogEntry>>
                        (jsonRes, opt)?? new List<LogEntry> { new LogEntry() };
                    return logs;
                }
                else
                {
                    throw new Exception($"로그 조회 실패: {jsonRes}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] GetLogMessageAsync 예외: {ex.Message}");
                throw;// 그대로 던짐
            }
        }
        public async Task<string> DiagnosisAsync(DiagnosisClassification cls , DiagnosisRegression reg)
        {
            try
            {
                var diagnosisPayload = new
                {
                    @class = cls,
                    regression = reg
                };
                string json = JsonSerializer.Serialize(diagnosisPayload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                /*var url = $"{BaseUrl}/{this.MyId}/logs";*/
                // 진단 완료 후 진단결과로 채팅을 요청함

                var url = $"{BaseUrl}/{this.MyId}/diagnosis";
                var res = await _httpClient.PostAsync(url,content);
                var jsonRes = await res.Content.ReadAsStringAsync();
                if (res.IsSuccessStatusCode)
                {
                    return jsonRes; 
                }
                else
                {
                    throw new Exception($"로그 조회 실패: {jsonRes}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] GetLogMessageAsync 예외: {ex.Message}");
                throw;// 그대로 던짐
            }
        }

        public async Task<string> SendUserSettingAsync(UserSettingPayload payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"{BaseUrl}/{MyId}/setting";

            var response = await _httpClient.PostAsync(url, content);
            var resContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[DEBUG] 사용자 설정 전송 성공: {resContent}");
            }
            else
            {
                Console.WriteLine($"[DEBUG] 전송 실패: {resContent}");
            }
            return resContent;
        }
        
        public string RequestTMapLatLon(double lat , double lon)
        {
            try
            {
  
                Console.WriteLine($"[DEBUG] 위치 좌표: lat = {lat}, lon = {lon}");
                return $"http://skinmate.shop/web/main.html?lat={lat}&lon={lon}";

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] 위치 조회 실패: {ex.Message}");
                return $"http://skinmate.shop/web/main.html?lat={lat}&lon={lon}";

            }
        }
        // app.cs에서 초기화 
        public async Task ContextInit()
        {
            try
            {
                var logs = await GetLogMessageAsync();
                _diaLogEntry = logs.Where(log => log.log_type == "진단분석").ToList();
                _chatLgoEntry = logs.Where(log => log.log_type == "질의응답").ToList();
                _userLogSetting = logs.Where(log => log.log_type == "사용자설정").FirstOrDefault();
                if (_diaLogEntry?.Any() == true)
                {
                    var latest = _diaLogEntry.Last(); // 가장 최신 진단
                    if (latest.diagnosis_result.HasValue)
                    {
                        var diagJson = latest.diagnosis_result.Value;
                        var classJson = diagJson.GetProperty("class").GetRawText();
                        var regressionJson = diagJson.GetProperty("regression").GetRawText();

                        var classData = JsonSerializer.Deserialize<DiagnosisClassification>(classJson);
                        var regressionData = JsonSerializer.Deserialize<DiagnosisRegression>(regressionJson);

                        DiagnosisDataStore.Instance.Update(classData, regressionData);
                        Console.WriteLine("[DEBUG] DiagnosisDataStore에 최신 진단 결과 업데이트 완료");
                    }
                }
                foreach (LogEntry log in logs)
                {
                    if (log.log_type == "진단분석")
                    {
                        // 진단 분석시에는 봇쪽에만 추가 
                        if (!string.IsNullOrEmpty(log.response))
                        {
                            string timePrefix = log.timestamp.ToString("yy-MM-dd HH:mm:ss")+"\n";
                            string concatMsg = string.Concat(timePrefix, log.response);
                            await ChatViewModel.Instance.AddBotMsg(concatMsg);  
                        }
                    }
                    else if (log.log_type == "질의응답")
                    {
                        if (!string.IsNullOrEmpty(log.response) && !string.IsNullOrEmpty(log.message))
                        {
                            string timePrefix = log.timestamp.ToString("yy-MM-dd HH:mm:ss") + "\n";
                            string concatMsg = string.Concat(timePrefix, log.message);
                            await ChatViewModel.Instance.AddUserMsg(concatMsg);  
                            concatMsg = string.Concat(timePrefix, log.response);
                            await ChatViewModel.Instance.AddBotMsg(concatMsg);
                        }

                    }
                    else if(log.log_type == "사용자설정")
                    {
                        if (!string.IsNullOrEmpty(log.response) && !string.IsNullOrEmpty(log.message))
                        {
                            string timePrefix = log.timestamp.ToString("yy-MM-dd HH:mm:ss") + "\n";
                          
                            _userSettingPayload = JsonSerializer.Deserialize<UserSettingPayload>(log.message);   
                            string concatMsg = string.Concat(timePrefix, log.response);
                            await ChatViewModel.Instance.AddBotMsg(concatMsg);
                        }
                    }
                }
                foreach (var entry in _diaLogEntry)
                {
                    if (!string.IsNullOrEmpty(entry.response))
                    {
                        Console.WriteLine(entry.response);
                    }
                    if (!string.IsNullOrEmpty(entry.message))
                    Console.WriteLine(JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true }));
                }
                Console.WriteLine("[DEBUG] 채팅 로그 수: " + _chatLgoEntry.Count);
                foreach (var entry in _chatLgoEntry)
                {
                    if (!string.IsNullOrEmpty(entry.response))
                    {
                        Console.WriteLine(entry.response);
                    }
                    Console.WriteLine(JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true }));
                }
                if (_userLogSetting != null)
                {
                    Console.WriteLine("[DEBUG] 유저 설정 로그: " + JsonSerializer.Serialize(_userLogSetting, new JsonSerializerOptions { WriteIndented = true }));
                }
                else
                {
                    Console.WriteLine("[DEBUG] 유저 설정 로그: 없음");
                }

            } 
            catch
            {
                throw new Exception($"Context Init 실패");
            }
        }
    }
}
