using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.Auth;

public record LoginRequestDto
(string Email,
  string Password
);