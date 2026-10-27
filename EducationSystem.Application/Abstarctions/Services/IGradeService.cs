using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Application.Dtos.Subject;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IGradeService
{
    Task<Result<IReadOnlyList<GradeResponse>>> GetAllAsync(CancellationToken ct = default);

    Task<Result<GradeResponse?>> GetByIdAsync(Guid Id, CancellationToken ct = default);

    //Task<Result<IReadOnlyList<GradeResponseDto>>> SearchAsync(string gradeName, CancellationToken ct = default);

    Task<Result<GradeResponse?>> AddAsync(CreateGradeRequest createDto, CancellationToken ct = default);

    Task<Result<GradeResponse?>> UpdateAsync(Guid Id, UpdateGradeRequest updateDto, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid Id, CancellationToken ct = default);

    Task<Result<IReadOnlyList<SubjectResponse>>> GetSubjectsAsync(Guid gradeId, CancellationToken ct = default);

    Task<Result<GradeSubjectResponse?>> AssignSubjectAsync(Guid gradeId, AssignSubjectToGradeRequest request, CancellationToken ct = default);

    Task<Result> RemoveSubjectAsync(Guid gradeId, Guid subjectId, CancellationToken ct = default);
}