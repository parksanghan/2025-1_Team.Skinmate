namespace AspRestServer.Dtos
{
    public record RegisterRequest(string UserId, string Password);
    public record LoginRequest(string UserId, string Password);

    // Dtos/SettingDtos.cs
}
