namespace EducationSystem.Application.Dtos.Auth;

public class JwtSetting
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public Double ExpiresIn { get; set; }
}