using EducationSystem.Application.Dtos.Auth;
using FluentValidation;

namespace EducationSystem.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.")
            .MaximumLength(256)
            .WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50)
            .WithMessage("Username must not exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage("Username can contain only letters, numbers, underscores, dots, and hyphens.");

        RuleFor(x => x.BirthDate)
            .Must(date => !date.HasValue || date.Value.Date < DateTime.UtcNow.Date)
            .WithMessage("Birth date must be in the past.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .MaximumLength(128)
            .WithMessage("Password must not exceed 128 characters.")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required.")
            .MaximumLength(250)
            .WithMessage("Address must not exceed 250 characters.");

        RuleFor(x => x.OrganisationId)
            .NotEqual(Guid.Empty)
            .When(x => x.OrganisationId.HasValue)
            .WithMessage("Organisation ID must be a valid GUID.");

        RuleFor(x => x.SchoolId)
            .NotEqual(Guid.Empty)
            .When(x => x.SchoolId.HasValue)
            .WithMessage("School ID must be a valid GUID.");

        RuleFor(x => x.GradeId)
            .NotEqual(Guid.Empty)
            .When(x => x.GradeId.HasValue)
            .WithMessage("Grade ID must be a valid GUID.");

        RuleFor(x => x.Role)
            .MaximumLength(50)
            .WithMessage("Role must not exceed 50 characters.")
            .When(x => x.Role is not null);
    }
}