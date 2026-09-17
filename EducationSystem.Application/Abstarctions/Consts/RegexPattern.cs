namespace EducationSystem.Application.Abstarctions.Consts;

public static class RegexPattern
{
    public const String Password
        = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{8,}$";
}