// DbManager.cs
// 필요 패키지: BCrypt.Net-Next (비밀번호 해시용)  -> dotnet add package BCrypt.Net-Next
// EF Core 컨텍스트/엔티티는 이미 제공하신 MyAppDbContext, User, ChatLog 사용

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using AspRestServer.Models;
using BCrypt.Net;

public class DbManager
{
    private readonly MyAppDbContext _db;
    private static readonly JsonSerializerOptions JsonOpt = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = false
    };

    public DbManager(MyAppDbContext db) => _db = db;

    // 1) 회원가입 (평문 저장 - 지양)
    public async Task<bool> RegisterUserAsync(string username, string password)
    {
        if (await _db.Users.AnyAsync(u => u.Username == username)) return false;

        _db.Users.Add(new User { Username = username, Password = password });
        await _db.SaveChangesAsync();
        return true;
    }

    // 2) 회원가입 (해시 저장 - 권장)
    public async Task<bool> RegisterUserHashedAsync(string username, string password)
    {
        if (await _db.Users.AnyAsync(u => u.Username == username)) return false;

        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        _db.Users.Add(new User { Username = username, Password = hashed });
        await _db.SaveChangesAsync();
        return true;
    }

    // 3) 로그인 (평문 비교 - 지양)
    public async Task<bool> LoginUserAsync(string username, string password)
    {
        var ok = await _db.Users
            .AnyAsync(u => u.Username == username && u.Password == password);
        return ok;
    }

    // 4) 로그인 (해시 검증 - 권장)
    public async Task<bool> LoginUserHashedAsync(string username, string password)
    {
        var user = await _db.Users
            .Where(u => u.Username == username)
            .Select(u => new { u.Password })
            .FirstOrDefaultAsync();

        if (user is null) return false;
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }

    // 5) 사용자 ID 조회
    public async Task<int?> GetUserIdAsync(string username)
    {
        var user = await _db.Users
            .Where(u => u.Username == username)
            .Select(u => new { u.UserId })
            .FirstOrDefaultAsync();

        return user?.UserId;
    }

    // 6) 질의응답 로그 추가
    public async Task<bool> AddChatLogAsync(string username, string question, string response)
    {
        var userId = await GetUserIdAsync(username);
        if (userId is null) return false;

        _db.ChatLogs.Add(new ChatLog
        {
            UserId = userId.Value,
            LogType = "질의응답",
            Message = question,
            Response = response,
            Timestamp = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return true;
    }

    // 7) 진단분석 로그 추가 (diagnosisResult: 익명객체/Dictionary/string(JSON) 모두 허용)
    public async Task<bool> AddDiagnosisLogAsync(string username, string imagePath, object diagnosisResult, string response)
    {
        var userId = await GetUserIdAsync(username);
        if (userId is null) return false;

        string diagJson = diagnosisResult switch
        {
            null => null!,
            string s => s,
            _ => JsonSerializer.Serialize(diagnosisResult, JsonOpt)
        };

        _db.ChatLogs.Add(new ChatLog
        {
            UserId = userId.Value,
            LogType = "진단분석",
            ImagePath = imagePath,
            DiagnosisResult = diagJson,
            Response = response,
            Timestamp = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return true;
    }

    // 8) 사용자설정 로그: 기존 한 건 있으면 업데이트, 없으면 신규 추가
    public async Task<bool> AddOrUpdateSettingLogAsync(string username, object data)
    {
        var userId = await GetUserIdAsync(username);
        if (userId is null) return false;

        string payloadJson = data switch
        {
            null => "{}",
            string s => s,
            _ => JsonSerializer.Serialize(data, JsonOpt)
        };

        var exists = await _db.ChatLogs
            .Where(c => c.UserId == userId.Value && c.LogType == "사용자설정")
            .OrderBy(c => c.ChatId)
            .FirstOrDefaultAsync();

        if (exists is not null)
        {
            exists.Message = payloadJson;
            exists.Response = "사용자 설정 업데이트 완료";
            // Timestamp는 기록 용도에 따라 유지/갱신 선택
            exists.Timestamp = DateTime.UtcNow;
        }
        else
        {
            _db.ChatLogs.Add(new ChatLog
            {
                UserId = userId.Value,
                LogType = "사용자설정",
                Message = payloadJson,
                Response = "사용자 설정 저장 완료",
                Timestamp = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    // 9) 사용자 로그 조회 (시간 오름차순 정렬)
    public async Task<List<ChatLog>> GetUserLogsAsync(string username)
    {
        var userId = await GetUserIdAsync(username);
        if (userId is null) return new List<ChatLog>();

        return await _db.ChatLogs
            .Where(c => c.UserId == userId.Value)
            .OrderBy(c => c.Timestamp)              // 제공하신 인덱스 IX_chat_logs_user_time와 정렬 일치
            .AsNoTracking()
            .ToListAsync();
    }
}
