using AspRestServer.Models;
using AspRestServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspRestServer.Controllers;

[ApiController]
[Route("{userId}/[controller]")]
public class ChatController : ControllerBase
{
    private readonly DbManager _db;
    private readonly OpenAiService _openai;

    public ChatController(DbManager db, OpenAiService openai)
    { _db = db; _openai = openai; }

    [HttpPost]
    public async Task<IActionResult> Chat(string userId, [FromForm] string message)
    {
        List<ChatLog> logs = await _db.GetUserLogsAsync(userId);
        
        var reply = await _openai.RequestChatResponseAsync(logs, message);
        await _db.AddChatLogAsync(userId, message, reply);
        return Ok(new { status = "ok", msg = reply });
    }
}
