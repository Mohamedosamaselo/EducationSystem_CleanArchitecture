using EducationSystem.Application.Abstarctions.Consts;
using EducationSystem.Application.Dtos.Request.Auth;
using FluentValidation;

namespace EducationSystem.Application.Validators.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
          .NotEmpty()
          .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPattern.Password)
            .WithMessage("Password must contain at least 8  characters, one uppercase letter, " +
                         "one lowercase letter, one digit, and one special character.");
        ;
    }
}