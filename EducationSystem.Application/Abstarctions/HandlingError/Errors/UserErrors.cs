using EducationSystem.Application.Abstarctions.HandlingError;

namespace EducationSystem.Application.Abstractions.HandlingError.Errors;

public static class UserErrors
{
    // Authentication errors
    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid email or password.");

    public static readonly Error UserNotFound =
        new("Auth.UserNotFound", "User was not found.");

    public static readonly Error EmailNotConfirmed =
        new("Auth.EmailNotConfirmed", "Email is not confirmed.");

    // Registration errors — distinct codes so MapErrorToStatus can route them correctly
    public static readonly Error DuplicateEmail =
        new("User.DuplicateEmail", "An account with this email already exists.");

    public static readonly Error DuplicateUserName =
        new("User.DuplicateUserName", "An account with this username already exists.");

    public static readonly Error WeakPassword =
        new("User.WeakPassword", "Password does not meet complexity requirements.");

    public static readonly Error CreationFailed =
        new("User.CreationFailed", "Failed to create the user account.");

    public static readonly Error RoleAssignmentFailed =
        new("User.RoleAssignmentFailed", "Failed to assign the default role.");
}