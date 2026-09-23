using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.Application.Abstarctions.HandlingError;

public static class ResultExtensions
{
    // Generic — for Result<T>
    // ─────────────────────────────────────────────────────────────
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase ctrl)
    {
        if (result.IsSuccess)
            return ctrl.Ok(result.Value);

        var status = MapErrorToStatus(result.Error.Code);
        var payload = new
        {
            code = result.Error.Code,
            description = result.Error.Description
        };

        return status switch
        {
            401 => ctrl.Unauthorized(payload),
            404 => ctrl.NotFound(payload),
            409 => ctrl.Conflict(payload),
            _ => ctrl.BadRequest(payload)
        };
    }

    // Non-generic — for Result (no value, e.g. Delete)   ⬅ ADD THIS
    // ─────────────────────────────────────────────────────────────
    public static IActionResult ToActionResult(this Result result, ControllerBase ctrl)
    {
        if (result.IsSuccess)
            return ctrl.NoContent();   // 204 No Content for deletes

        var payload = new
        {
            code = result.Error.Code,
            description = result.Error.Description
        };

        return MapErrorToStatus(result.Error.Code) switch
        {
            401 => ctrl.Unauthorized(payload),
            404 => ctrl.NotFound(payload),
            409 => ctrl.Conflict(payload),
            500 => ctrl.StatusCode(500, payload),
            _ => ctrl.BadRequest(payload)
        };
    }

    private static int MapErrorToStatus(string code) => code switch
    {
        // Auth errors
        "Auth.InvalidCredentials" => 401,
        "Auth.UserNotFound" => 404,
        "Auth.EmailNotConfirmed" => 401,

        // User errors
        "User.NotFound" => 404,
        "User.DuplicateEmail" => 409,
        "User.DuplicateUserName" => 409,
        "User.WeakPassword" => 400,
        "User.CreationFailed" => 400,

        // Organisation errors
        "Organisation.NotFound" => 404,
        "Organisation.DuplicateEmail" => 409,
        "Organisation.DuplicateName" => 409,
        "Organisation.InvalidInput" => 400,
        "Organisation.CreationFailed" => 500,
        "Organisation.UpdateFailed" => 500,
        "Organisation.DeletionFailed" => 500,

        // School errors
        "School.NotFound" => 404,
        "School.OrganisationNotFound" => 404,
        "School.DuplicateEmail" => 409,
        "School.DuplicateName" => 409,
        "School.InvalidInput" => 400,
        "School.CreationFailed" => 500,
        "School.UpdateFailed" => 500,
        "School.DeletionFailed" => 500,

        _ => 400
    };
}