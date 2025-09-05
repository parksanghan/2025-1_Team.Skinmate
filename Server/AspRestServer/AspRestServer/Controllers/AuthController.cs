using AspRestServer.Dtos;
using AspRestServer.Models;
using Microsoft.AspNetCore.Mvc;
using AspRestServer.Services; // ✅ 추가
namespace AspRestServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DbManager _db;

    public AuthController(DbManager db) => _db = db;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var ok = await _db.LoginUserHashedAsync(req.UserId, req.Password);
        if (!ok) return Unauthorized(new { status = "invalid" });
        return Ok(new { status = "ok" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var ok = await _db.RegisterUserHashedAsync(req.UserId, req.Password);
        if (!ok) return Conflict(new { status = "dup" });
        return Ok(new { status = "ok" });
    }
}
