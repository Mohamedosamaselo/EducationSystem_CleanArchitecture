using EducationSystem.Application.Dtos;
using FluentValidation;

namespace EducationSystem.Application.Validators.Identity;

public class ResendConfirmationEmailRequestValidator
    : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");
    }
}