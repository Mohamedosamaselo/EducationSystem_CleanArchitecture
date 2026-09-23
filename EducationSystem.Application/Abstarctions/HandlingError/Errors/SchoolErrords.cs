namespace EducationSystem.Application.Abstarctions.HandlingError.Errors;

public static class SchoolErrors
{
    public static readonly Error NotFound =
        new("School.NotFound", "School was not found.");

    public static readonly Error OrganisationNotFound =
        new("School.OrganisationNotFound", "The referenced organisation does not exist.");

    public static readonly Error DuplicateEmail =
        new("School.DuplicateEmail", "A school with this email already exists.");

    public static readonly Error DuplicateName =
        new("School.DuplicateName", "A school with this name already exists.");

    public static readonly Error InvalidInput =
        new("School.InvalidInput", "The provided school data is invalid.");

    public static readonly Error CreationFailed =
        new("School.CreationFailed", "Failed to create the school.");

    public static readonly Error UpdateFailed =
        new("School.UpdateFailed", "Failed to update the school.");

    public static readonly Error DeletionFailed =
        new("School.DeletionFailed", "Failed to delete the school.");
}