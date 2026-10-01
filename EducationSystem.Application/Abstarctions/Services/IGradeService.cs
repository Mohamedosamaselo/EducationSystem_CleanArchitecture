using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.Grade;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IGradeService
{
    Task<Result<IReadOnlyList<GradeResponseDto>>> GetAllAsync(CancellationToken ct = default);

    Task<Result<GradeResponseDto?>> GetByIdAsync(Guid Id, CancellationToken ct = default);

    Task<Result<IReadOnlyList<GradeResponseDto>>> SearchAsync(string gradeName, CancellationToken ct = default);

    Task<Result<GradeResponseDto?>> CreateAsync(CreateGradeRequest createDto, CancellationToken ct = default);

    Task<Result<GradeResponseDto?>> UpdateAsync(Guid Id, UpdateGradeRequest updateDto, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid Id, CancellationToken ct = default);
}