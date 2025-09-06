// Dtos/AuthDtos.cs
namespace AspRestServer.Dtos;

 
public class UserSettingPayload
{
    public string[] Interests { get; set; } = Array.Empty<string>();
    public string Gender { get; set; } = "";
    public string Age { get; set; } = "";
}
