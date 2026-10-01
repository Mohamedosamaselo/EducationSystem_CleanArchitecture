using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.Subject;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISubjectService
{
    Task<SubjectResponseDto?> GetSubjectAsync(Guid id, CancellationToken CT = default);

    Task<SubjectResponseDto?> GetByNameAsync(string subjectName, CancellationToken CT = default);// Search by Subject Name

    Task<IReadOnlyList<SubjectResponseDto>> GetAllSubjectsAsync(CancellationToken CT = default);

    Task<SubjectResponseDto> CreateSubjectAsync(CreateSubjectRequest subjectRequest, CancellationToken CT = default);

    Task<SubjectResponseDto?> UpdateSubjectAsync(Guid id, UpdateSubjectRequest subjectRequest, CancellationToken CT = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken CT = default);
}