using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.HandlingError.Errors;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Application.Dtos.Subject;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Services;

public class SubjectService(IUnitOfWork unitOfWork) : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<SubjectResponse>> GetAllAsync(CancellationToken CT = default)
    {
        // loaad Subject with Grades , School Navigational Property
        var subjects = await _unitOfWork.SubjectRepository.GetAllWithIncludesAsync(null, CT, s => s.Grades);

        if (subjects == null || !subjects.Any())
            return new List<SubjectResponse>();

        return subjects.Select(s => new SubjectResponse
        {
            Name = s.Name,
            Grades = s.Grades.Select(g => new GradeResponse
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                //IsActive = g.IsActive,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        }).ToList();
    }

    public async Task<SubjectResponse?> GetByIdAsync(Guid id, CancellationToken CT = default)
    {
        // loaad Subject with Grades , School Navigational Property
        var subject = await _unitOfWork.SubjectRepository
                                        .GetByIdWithIncludeAsync(id,
                                                                 CT,
                                                                 s => s.Grades
                                                                 //s => s.School
                                                                 );

        if (subject == null)
            throw new Exception("no subject");

        return new SubjectResponse
        {
            Name = subject.Name,
            Grades = subject.Grades.Select(g => new GradeResponse
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                //IsActive = g.IsActive,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        };
    }

    //public async Task<SubjectResponse?> GetByNameAsync(string subjectName, CancellationToken CT = default)
    //{
    //    var subject = await _unitOfWork.SubjectRepository.GetByNameAsync(subjectName, CT);

    //    if (subject == null)
    //        throw new Exception("No Subjects ");

    //    return new SubjectResponse
    //    {
    //        //Id = subject.Id,
    //        Name = subject.Name,
    //        //Description = subject.Description,

    //        //SchoolId = subject.SchoolId,
    //        //SchoolName = subject.School.Name,

    //        Grades = subject.Grades.Select(g => new GradeResponse
    //        {
    //            Id = g.Id,
    //            Name = g.Name,
    //            Description = g.Description,
    //            //IsActive = g.IsActive,
    //            SchoolId = g.SchoolId,
    //            SchoolName = g.School?.Name ?? string.Empty
    //        }).ToList()
    //    };
    //}

    public async Task<SubjectResponse> AddAsync(CreateSubjectRequest dto, CancellationToken ct = default)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Name))
            throw new Exception("Invalid Subject data");

        var name = dto.Name.Trim();

        var duplicate = await _unitOfWork.SubjectRepository.FirstOrDefaultAsync(s => s.Name == name, ct);
        if (duplicate is not null)
            throw new Exception("Subject with the same name already exists");

        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Name = name
        };
        await _unitOfWork.SubjectRepository.AddAsync(subject, ct);

        var grade = await _unitOfWork.GradeRepository.GetByIdAsync(dto.GradeId, ct);
        if (grade == null)
            throw new Exception("Grade not found");

        subject.Grades.Add(grade);

        await _unitOfWork.SubjectRepository.AddAsync(subject, ct);

        try { await _unitOfWork.SaveChangesAsync(ct); }
        catch (Exception) { throw new Exception("Failed to save subject"); }

        return new SubjectResponse
        {
            Name = subject.Name,
            Grades = subject.Grades.Select(g => new GradeResponse
            {
                Id = g.Id,
                Name = g.Name,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        };
    }

    public async Task<SubjectResponse?> UpdateAsync(Guid id,
            UpdateSubjectRequest dto, CancellationToken ct = default)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Name))
            throw new Exception("Invalid Subject data");

        var name = dto.Name.Trim();

        var duplicate = await _unitOfWork.SubjectRepository
            .FirstOrDefaultAsync(s => s.Name == name, ct);
        if (duplicate is not null)
            throw new Exception("Subject with the same name already exists");

        var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(id, ct);
        if (subject == null)
            throw new Exception("no Subject Found");

        subject.Name = dto.Name;
        subject.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.SubjectRepository.Update(subject);

        try { await _unitOfWork.SaveChangesAsync(ct); }
        catch (Exception) { throw new Exception("Failed to update subject"); }

        return new SubjectResponse
        {
            Name = subject.Name,
            Grades = subject.Grades.Select(g => new GradeResponse
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                SchoolId = g.SchoolId,
                SchoolName = g.School?.Name ?? string.Empty
            }).ToList()
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            throw new Exception("Invalid subject ID");

        // Load with its grades so we can block deleting a subject that is in use
        var subject = await _unitOfWork.SubjectRepository
            .GetByIdWithIncludeAsync(id, ct, s => s.Grades);
        if (subject is null)
            throw new Exception("Subject not found");

        if (subject.Grades.Any())
            throw new Exception("Cannot delete subject that is assigned to grades");

        _unitOfWork.SubjectRepository.Delete(subject);

        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}