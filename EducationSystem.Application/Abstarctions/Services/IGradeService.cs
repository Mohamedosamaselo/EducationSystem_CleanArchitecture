using EducationSystem.Application.Dtos.Request.Grade;
using EducationSystem.Application.Dtos.Response.Grade;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IGradeService
{
    Task<IReadOnlyList<GradeResponseDto>> GetAllAsync();

    Task<GradeResponseDto?> GetByIdAsync(Guid Id);

    Task<IReadOnlyList<GradeResponseDto>> SearchAsync(string gradeName);

    Task<GradeResponseDto> CreateAsync(CreateGradeRequest createRequestDto);

    Task<GradeResponseDto?> UpdateAsync(Guid Id, UpdateGradeRequest updateRequestDto);

    Task<bool> DeleteAsync(Guid Id);
}