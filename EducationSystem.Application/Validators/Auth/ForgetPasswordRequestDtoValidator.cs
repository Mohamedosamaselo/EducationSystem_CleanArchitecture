using EducationSystem.Application.Dtos.Request;
using FluentValidation;

namespace EducationSystem.Application.Validators.Auth;

public class ForgetPasswordRequestDtoValidator : AbstractValidator<ForgetPasswordRequestDto>
{
    public ForgetPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Email)
       .NotEmpty()
       .EmailAddress();
    }
}