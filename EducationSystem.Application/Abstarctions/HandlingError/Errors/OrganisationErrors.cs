// EducationSystem.Application/Abstractions/HandlingError/Errors/OrganisationErrors.cs

using EducationSystem.Application.Abstarctions.HandlingError;

namespace EducationSystem.Application.Abstractions.HandlingError.Errors;

public static class OrganisationErrors
{
    public static readonly Error NotFound =
        new("Organisation.NotFound", "Organisation was not found.");

    public static readonly Error DuplicateEmail =
        new("Organisation.DuplicateEmail", "An organisation with this email already exists.");

    public static readonly Error DuplicateName =
        new("Organisation.DuplicateName", "An organisation with this name already exists.");

    public static readonly Error InvalidInput =
        new("Organisation.InvalidInput", "The provided organisation data is invalid.");

    public static readonly Error CreationFailed =
        new("Organisation.CreationFailed", "Failed to create the organisation.");

    public static readonly Error UpdateFailed =
        new("Organisation.UpdateFailed", "Failed to update the organisation.");

    public static readonly Error DeletionFailed =
        new("Organisation.DeletionFailed", "Failed to delete the organisation.");
}