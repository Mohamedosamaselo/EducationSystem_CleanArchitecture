using EducationSystem.Application.Dtos.Subject;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISubjectService
{
    Task<IReadOnlyList<SubjectResponse>> GetAllAsync(CancellationToken CT = default);

    Task<SubjectResponse?> GetByIdAsync(Guid id, CancellationToken CT = default);

    //Task<SubjectResponse?> GetByNameAsync(string subjectName, CancellationToken CT = default);// Search by Subject Name

    Task<SubjectResponse> AddAsync(CreateSubjectRequest subjectRequest, CancellationToken CT = default);

    Task<SubjectResponse?> UpdateAsync(Guid id, UpdateSubjectRequest subjectRequest, CancellationToken CT = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken CT = default);
}