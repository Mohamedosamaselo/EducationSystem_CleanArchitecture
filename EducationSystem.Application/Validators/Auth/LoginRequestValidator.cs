using EducationSystem.Application.Dtos.Request.Auth;
using EducationSystem.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Password).NotEmpty();
    }
}