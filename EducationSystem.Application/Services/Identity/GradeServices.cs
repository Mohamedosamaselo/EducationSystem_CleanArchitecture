using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Request.Grade;
using EducationSystem.Application.Dtos.Response.Grade;
using EducationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace EducationSystem.Application.Services.Identity;

public class GradeServices(IUnitOfWork unitOfWork) : IGradeService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<GradeResponseDto>> GetAllAsync()
    {
        // load navigational properties [Subject , school , users ] eager with grades
        var grades = await _unitOfWork.GradeRepository.GetAllWithIncludesAsync(null,
                         p => p.Subjects,
                         p => p.School,
                         p => p.Users
                            );

        if (grades == null || !grades.Any())
            throw new Exception("No grades found.");

        var gradeDtos = grades.Select(g => new GradeResponseDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            IsActive = g.IsActive,
            SchoolId = g.SchoolId,
            SchoolName = g.School?.Name ?? string.Empty,
            //SubjectsCount = g.Subjects?.Count ?? 0,
            //StudentsCount = g.Users?.Count ?? 0
        }).ToList();

        return gradeDtos;
    }

    public async Task<GradeResponseDto?> GetByIdAsync(Guid Id)
    {
        // load navigational properties [Subject , school , users ] eager with grades
        var grades = await _unitOfWork.GradeRepository.GetByIdWithIncludeAsync(Id,
                         p => p.Subjects,
                         p => p.School,
                         p => p.Users
                            );

        if (grades == null)
            throw new Exception("No grades found.");

        var gradeDtos = new GradeResponseDto
        {
            Id = Id,
            Name = grades.Name,
            Description = grades.Description,
            IsActive = grades.IsActive,
            SchoolId = grades.SchoolId,
            SchoolName = grades.School?.Name ?? string.Empty,
            //SubjectsCount = grades.Subjects?.Count ?? 0,
            //StudentsCount = grades.Users?.Count ?? 0
        };

        return gradeDtos;
    }

    public async Task<IReadOnlyList<GradeResponseDto>> SearchAsync(string gradeName)
    {
        var grades = await _unitOfWork.GradeRepository.GetAllWithIncludesAsync(g => g.Name.ToLower().Contains(gradeName.ToLower()),
                         p => p.Subjects,
                         p => p.School,
                         p => p.Users
                            );

        if (grades == null || !grades.Any())
            throw new Exception("No grades found.");

        var gradeDtos = grades.Select(g => new GradeResponseDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            IsActive = g.IsActive,
            SchoolId = g.SchoolId,
            SchoolName = g.School?.Name ?? string.Empty,
            //SubjectsCount = g.Subjects?.Count ?? 0,
            //StudentsCount = g.Users?.Count ?? 0
        }).ToList();

        return gradeDtos;
    }

    public async Task<GradeResponseDto> CreateAsync(CreateGradeRequest createRequestDto)
    {
        var grade = new Grade
        {
            Name = createRequestDto.Name,
            Description = createRequestDto.Description,
            SchoolId = createRequestDto.SchoolId
        };

        await _unitOfWork.GradeRepository.AddAsync(grade);

        await _unitOfWork.SaveChangesAsync();

        var gradeResponseDto = new GradeResponseDto
        {
            Id = grade.Id,
            Name = grade.Name,
            Description = grade.Description,
            IsActive = grade.IsActive,
            SchoolId = grade.SchoolId,
            SchoolName = (await _unitOfWork.SchoolRepository.GetByIdAsync(grade.SchoolId))?.Name ?? string.Empty
        };
        return gradeResponseDto;
    }

    public async Task<GradeResponseDto?> UpdateAsync(Guid Id, UpdateGradeRequest updateRequestDto)
    {
        var grade = await _unitOfWork.GradeRepository.GetByIdAsync(Id);
        if (grade == null)
            throw new Exception("Grade not found.");

        grade.Name = updateRequestDto.Name;
        grade.Description = updateRequestDto.Description;
        grade.IsActive = updateRequestDto.IsActive;

        await _unitOfWork.SaveChangesAsync();

        var gradeResponseDto = new GradeResponseDto
        {
            Id = grade.Id,
            Name = grade.Name,
            Description = grade.Description,
            IsActive = grade.IsActive,
            SchoolId = grade.SchoolId,
            SchoolName = (await _unitOfWork.SchoolRepository.GetByIdAsync(grade.SchoolId))?.Name ?? string.Empty
        };
        return gradeResponseDto;
    }

    public async Task<GradeResponseDto?> DeleteAsync(Guid Id)
    {
        var grade = await _unitOfWork.GradeRepository.GetByIdAsync(Id);
        if (grade == null)
            throw new Exception("Grade not found.");

        _unitOfWork.GradeRepository.Delete(grade);

        await _unitOfWork.SaveChangesAsync();

        var gradeResponseDto = new GradeResponseDto
        {
            Id = grade.Id,
            Name = grade.Name,
            Description = grade.Description,
            IsActive = grade.IsActive,
            SchoolId = grade.SchoolId,
            SchoolName = (await _unitOfWork.SchoolRepository.GetByIdAsync(grade.SchoolId))?.Name ?? string.Empty
        };
        return gradeResponseDto;
    }
}