using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.Request;

public record LoginRequestDto
(string Email,
  string Password
);