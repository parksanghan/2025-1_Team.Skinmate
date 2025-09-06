// Controllers/UploadController.cs
using System.Net.Http.Headers;
using System.Text.Json;
using AspRestServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspRestServer.Controllers;

[ApiController]
[Route("{userId}/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly DbManager _db;           // 선택: 로그 저장
    private readonly OpenAiService _openai;   // 선택: 설명 생성

    public UploadController(IHttpClientFactory httpFactory, DbManager db, OpenAiService openai)
    {
        _httpFactory = httpFactory;
        _db = db;
        _openai = openai;
    }

    /// <summary>
    /// 실제 추론 서버 포워딩: /{userId}/Upload/upload33
    /// 멀티파트 form-data로 첫 번째 파일을 "image" 키로 전송 -> 응답 JSON 그대로 반환
    /// </summary>
    [HttpPost("upload")]
    [RequestSizeLimit(20_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadForward(string userId, List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
            return BadRequest(new { status = "fail", msg = "No files processed" });

        var file = files[0];
        var client = _httpFactory.CreateClient();

        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content.Add(streamContent, "image", file.FileName); // FastAPI 코드와 동일하게 "image" 키 사용

        var endpoint = "http://182.210.98.131:5000/diagnose";
        using var res = await client.PostAsync(endpoint, content);
        var body = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
        {
            // 실패 시 원문 body를 함께 전달
            return StatusCode((int)res.StatusCode,
                new { status = "error", msg = $"추론 서버 오류: {(int)res.StatusCode}", detail = body });
        }

        // 응답을 JSON으로 파싱 (원문 유지)
        JsonElement diagnosisJson;
        try
        {
            diagnosisJson = JsonSerializer.Deserialize<JsonElement>(body);
        }
        catch
        {
            // JSON이 아닐 경우, 원문 스트링으로 반환
            return Ok(new
            {
                status = "ok",
                msg = "진단 서버 응답 출력 완료(원문)",
                diagnosis_result_raw = body
            });
        }

        // (선택) OpenAI로 “사람에게 설명” 생성 + DB 로그 저장
        // 필요없으면 이 블록 삭제 가능
        string explanation;
        try
        {
            var logs = await _db.GetUserLogsAsync(userId);
            explanation = await _openai.RequestChatDiagnosisAsync(logs, diagnosisJson);
            await _db.AddDiagnosisLogAsync(userId, imagePath: null, diagnosisResult: diagnosisJson, response: explanation);
        }
        catch
        {
            // 설명/로그 저장 실패해도 진단 결과는 반환
            explanation = "설명 생성 또는 로그 저장 중 문제가 발생했지만, 진단 결과는 정상 수신되었습니다.";
        }

        return Ok(new
        {
            status = "ok",
            msg = "진단 서버 응답 출력 완료",
            diagnosis_result = diagnosisJson,
            explanation
        });
    }
}
