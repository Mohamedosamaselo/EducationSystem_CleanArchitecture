using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Dtos.School;

public class SchoolGradeStudentResponseDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }
}