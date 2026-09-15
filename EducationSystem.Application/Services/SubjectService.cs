using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Request.Subject;
using EducationSystem.Application.Dtos.Response.Grade;
using EducationSystem.Application.Dtos.Response.Subject;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Services;

public class SubjectService(IUnitOfWork unitOfWork) : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<SubjectResponseDto>> GetAllSubjectsAsync()
    {
        // loaad Subject with Grades , School Navigational Property
        var subjects = await _unitOfWork.SubjectRepository.GetAllWithIncludesAsync(null,
            s => s.Grades,
            s => s.School);

        if (subjects == null || !subjects.Any())
            return new List<SubjectResponseDto>();

        return subjects.Select(s => new SubjectResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,

            SchoolId = s.SchoolId,
            SchoolName = s.School.Name,

            Grades = s.Grades.Select(g => new GradeResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsActive = g.IsActive,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        }).ToList();
    }

    public async Task<SubjectResponseDto?> GetSubjectAsync(Guid id)
    {
        // loaad Subject with Grades , School Navigational Property
        var subject = await _unitOfWork.SubjectRepository
            .GetByIdWithIncludeAsync(id,
                                     s => s.Grades,
                                     s => s.School);

        if (subject == null)
            throw new Exception("no subject");

        return new SubjectResponseDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Description = subject.Description,

            SchoolId = subject.SchoolId,
            SchoolName = subject.School.Name,

            Grades = subject.Grades.Select(g => new GradeResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsActive = g.IsActive,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        };
    }

    public async Task<SubjectResponseDto?> GetByNameAsync(string subjectName)
    {
        var subject = await _unitOfWork.SubjectRepository.GetByNameAsync(subjectName);

        if (subject == null)
            throw new Exception("No Subjects ");

        return new SubjectResponseDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Description = subject.Description,

            SchoolId = subject.SchoolId,
            SchoolName = subject.School.Name,

            Grades = subject.Grades.Select(g => new GradeResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsActive = g.IsActive,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        };
    }

    public async Task<SubjectResponseDto> CreateSubjectAsync(CreateSubjectRequest subjectRequest)
    {
        var newSubject = new Subject
        {
            Name = subjectRequest.Name,
            Description = subjectRequest.Description,
            SchoolId = subjectRequest.SchoolId,
        };

        var grade = await _unitOfWork.GradeRepository.GetByIdAsync(subjectRequest.GradeId);

        if (grade == null)
            throw new Exception("Grade not found");

        newSubject.Grades.Add(grade);

        await _unitOfWork.SubjectRepository.AddAsync(newSubject);

        await _unitOfWork.SaveChangesAsync();

        return new SubjectResponseDto
        {
            Name = newSubject.Name,
            Description = newSubject.Description,
            SchoolId = newSubject.SchoolId,
            Grades = newSubject.Grades.Select(g => new GradeResponseDto
            {
                //Id = g.Id,
                Name = g.Name,
                //Description = g.Description,
                //IsActive = g.IsActive,
                //SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        };
    }

    public async Task<SubjectResponseDto?> UpdateSubjectAsync(Guid id,
        UpdateSubjectRequest subjectRequest)
    {
        var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(id);

        if (subject == null)
            throw new Exception("no Subject Found");

        subject.Name = subjectRequest.Name;
        subject.Description = subjectRequest.Description;
        subject.SchoolId = subjectRequest.SchoolId;

        _unitOfWork.SubjectRepository.Update(subject);
        await _unitOfWork.SaveChangesAsync();

        return new SubjectResponseDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Description = subject.Description,
            SchoolId = subject.SchoolId,
            SchoolName = subject.School?.Name ?? string.Empty,
            Grades = subject.Grades.Select(g => new GradeResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsActive = g.IsActive,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(id);
        if (subject == null)
            return false;

        _unitOfWork.SubjectRepository.Delete(subject);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}