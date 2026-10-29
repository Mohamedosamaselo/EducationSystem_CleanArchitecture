using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Dtos.Teacher;

public sealed record TeacherStudentResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
);