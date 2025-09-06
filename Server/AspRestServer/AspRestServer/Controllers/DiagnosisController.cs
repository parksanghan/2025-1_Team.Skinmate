using AspRestServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspRestServer.Controllers;

[ApiController]
[Route("{userId}/[controller]")]
public class DiagnosisController : ControllerBase
{
    private readonly DbManager _db;
    private readonly OpenAiService _openai;

    public DiagnosisController(DbManager db, OpenAiService openai)
    { _db = db; _openai = openai; }

    public class DiagnosisPayload { public object? Data { get; set; } }

    [HttpPost]
    public async Task<IActionResult> Explain(string userId, [FromBody] object diagnosis)
    {
        var logs = await _db.GetUserLogsAsync(userId);
        var text = await _openai.RequestChatDiagnosisAsync(logs, diagnosis);
        await _db.AddDiagnosisLogAsync(userId, imagePath: null, diagnosisResult: diagnosis, response: text);
        return Ok(text);
    }
}
