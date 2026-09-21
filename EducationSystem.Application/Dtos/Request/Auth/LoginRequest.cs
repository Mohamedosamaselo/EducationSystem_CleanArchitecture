namespace EducationSystem.Application.Dtos.Request.Auth;

public record LoginRequest
(
  string Email,
  string Password
);