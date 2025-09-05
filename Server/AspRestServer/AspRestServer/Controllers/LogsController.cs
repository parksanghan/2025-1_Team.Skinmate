using Microsoft.AspNetCore.Mvc;

namespace AspRestServer.Controllers;

[ApiController]
[Route("{userId}/[controller]")]
public class LogsController : ControllerBase
{
    private readonly DbManager _db;
    public LogsController(DbManager db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get(string userId)
    {
        var logs = await _db.GetUserLogsAsync(userId);
        return Ok(logs);
    }
}
