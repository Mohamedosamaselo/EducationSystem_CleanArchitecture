using EducationSystem.Application.Dtos;
using FluentValidation;

namespace EducationSystem.Application.Validators.Auth;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required.");
    }
}