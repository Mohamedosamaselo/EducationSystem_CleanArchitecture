namespace EducationSystem.Application.Dtos.Auth;

public class AuthModel
{
    public string Message { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
    public string UserToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}