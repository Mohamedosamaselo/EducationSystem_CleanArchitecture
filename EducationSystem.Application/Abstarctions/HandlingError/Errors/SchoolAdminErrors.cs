namespace EducationSystem.Application.Abstarctions.HandlingError.Errors;

public static class SchoolAdminErrors
{
    public static readonly Error InvalidInput =
        new("SchoolAdmin.InvalidInput", "Invalid input.");

    public static readonly Error NotFound =
        new("SchoolAdmin.NotFound", "This school has no admin.");

    public static readonly Error AlreadyExists =
        new("SchoolAdmin.AlreadyExists", "This school already has an admin. Edit or remove it first.");

    public static readonly Error DuplicateUser =
      new("SchoolAdmin.DuplicateUser", "User is already used.");

    public static readonly Error DuplicateUsername =
        new("SchoolAdmin.DuplicateUsername", "Username  is already used.");

    public static readonly Error DuplicateEmail =
       new("SchoolAdmin.DuplicateEmail", "Email is already used.");

    public static readonly Error RoleNotConfigured =
        new("SchoolAdmin.RoleNotConfigured", "The SchoolAdmin role does not exist.");

    public static readonly Error CreationFailed =
        new("SchoolAdmin.CreationFailed", "Failed to create the school admin.");

    public static readonly Error UpdateFailed =
        new("SchoolAdmin.UpdateFailed", "Failed to update the school admin.");

    public static readonly Error DeletionFailed =
        new("SchoolAdmin.DeletionFailed", "Failed to remove the school admin.");
}