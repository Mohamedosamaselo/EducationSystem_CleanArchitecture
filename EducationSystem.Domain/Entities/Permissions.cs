using EducationSystem.Domain.Entities.Common;

namespace EducationSystem.Domain.Entities;

public class Permissions : BaseAuditableEntity
{
    // Schools

    public const string GetSchools = "schools:read";
    public const string AddSchools = "schools:add";
    public const string UpdateSchools = "schools:update";
    public const string DeleteSchools = "schools:delete";

    // Grades

    public const string GetGrades = "grades:read";
    public const string AddGrades = "grades:add";
    public const string UpdateGrades = "grades:update";
    public const string DeleteGrades = "grades:delete";

    // Subjects

    public const string GetSubjects = "subjects:read";
    public const string AddSubjects = "subjects:add";
    public const string UpdateSubjects = "subjects:update";
    public const string DeleteSubjects = "subjects:delete";
}