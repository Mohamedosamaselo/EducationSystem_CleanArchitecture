using EducationSystem.Application.Abstarctions.Consts;
using EducationSystem.Application.Dtos.Request;
using FluentValidation;

namespace EducationSystem.Application.Validators.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("Password must contain at least 6 characters, one uppercase letter, " +
                         "one lowercase letter, one digit, and one special character.");

        RuleFor(x => x.SchoolId)
            .NotEmpty();

        RuleFor(x => x.GradeId)
            .NotEmpty();
    }
}