using AspRestServer.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AspRestServer.Controllers;

[ApiController]
[Route("{userId}/[controller]")]
public class SettingController : ControllerBase
{
    private readonly DbManager _db;
    public SettingController(DbManager db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Save(string userId, [FromBody] UserSettingPayload req)
    {
        await _db.AddOrUpdateSettingLogAsync(userId, req);
        return Ok("사용자설정이 완료되었습니다.");
    }
}
