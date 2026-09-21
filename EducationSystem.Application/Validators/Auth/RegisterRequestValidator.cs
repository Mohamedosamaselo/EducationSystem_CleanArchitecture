using EducationSystem.Application.Dtos.Request.Auth;
using FluentValidation;

namespace EducationSystem.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
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
            .WithMessage("Password Atleast contains 8 digits and required ");

        RuleFor(x => x.Address)
         .NotEmpty()
         .MaximumLength(400);

        ;
    }
}