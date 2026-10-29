using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Dtos.Teacher;

public sealed record TeacherEvaluationResponseDto(
    Guid StudentId,
    string StudentName,
    Guid SubjectId,
    string SubjectName,
    string? FinalEvaluation
);