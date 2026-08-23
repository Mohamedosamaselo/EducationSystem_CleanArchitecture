using EducationSystem.Application.Abstarctions.Consts;
using EducationSystem.Application.Dtos.Auth;
using EducationSystem.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Application.Validators.Auth;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginRequestDtoValidator(UserManager<ApplicationUser> userManager)
    {
        RuleFor(x => x.Email)
       .NotEmpty()
       .EmailAddress();

        RuleFor(x => x.Password)
             .NotEmpty()
             .Matches(RegexPattern.Password)
             .WithMessage("Password must contain at least 6 characters, one uppercase letter, " +
                 "one lowercase letter, one digit, and one special character.");
        this._userManager = userManager;
    }
}