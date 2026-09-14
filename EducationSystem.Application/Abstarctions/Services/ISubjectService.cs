using EducationSystem.Application.Dtos.Request.Subject;
using EducationSystem.Application.Dtos.Response;
using EducationSystem.Application.Dtos.Response.Subject;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISubjectService
{
    Task<SubjectResponseDto?> GetSubjectAsync(Guid id);

    Task<SubjectResponseDto?> GetByNameAsync(string subjectName);// Search by Subject Name

    Task<IReadOnlyList<SubjectResponseDto>> GetAllSubjectsAsync();

    Task<SubjectResponseDto> CreateSubjectAsync(CreateSubjectRequest subjectRequest);

    Task<SubjectResponseDto?> UpdateSubjectAsync(Guid id, UpdateSubjectRequest subjectRequest);

    Task<bool> DeleteAsync(Guid id);
}