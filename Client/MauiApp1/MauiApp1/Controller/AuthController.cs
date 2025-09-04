using MauiApp1.Model;
using MauiApp1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp1.Controller
{
    public class AuthController
    {

        private readonly HttpClient _httpClient = HttpClientProvider.Instance;
        private const string BaseUrl = "http://192.168.0.10:8080";
       
        public async Task<string> LoginAsync(LoginRequest request)
        {
            var url = "http://192.168.0.10:8080/login"; // 실제 서버 IP로 변경
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json,Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync(url, content);
            if (res.IsSuccessStatusCode)
            {
                //var result = JsonSerializer.Deserialize<LoginResponse>(responseJson);
                return await res.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"서버 오류: {await res.Content.ReadAsStringAsync()}");
            }
        }
        public async Task<String> RegisterAsync(RegisterRequest request)
        {
            var url = "http://192.168.0.10:8080/register"; // 실제 서버 IP로 변경
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync(url, content);
            if (res.IsSuccessStatusCode)
            {
                //var result = JsonSerializer.Deserialize<LoginResponse>(responseJson);
                return await res.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"서버 오류: {await res.Content.ReadAsStringAsync()}");
            }
        }


  
    }
}
