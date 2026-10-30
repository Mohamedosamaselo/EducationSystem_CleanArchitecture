using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Abstarctions.Persistence.Repositories;

public interface IGradeRepo : IGenericRepository<Grade>
{
    Task<bool> SchoolHasSubjectAsync(
        Guid schoolId,
        Guid subjectId,
        CancellationToken ct = default);

    Task<bool> GradeHasSubjectAsync(
        Guid gradeId,
        Guid schoolId,
        Guid subjectId,
        CancellationToken ct = default);

    Task<IReadOnlyList<Grade>> GetTeacherGradesAsync(
        Guid teacherId,
        Guid schoolId,
        CancellationToken ct = default);
}