using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Services;

public class GradeServices(IUnitOfWork unitOfWork) : IGradeService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<GradeResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        // load navigational properties [Subject , school , users ] eager with grades
        var grades = await _unitOfWork.GradeRepository.GetAllWithIncludesAsync(null, ct,
                         p => p.Subjects,
                         p => p.School,
                         p => p.Users
                            );

        // If there are no grades, return a successful Result with an empty list.
        if (grades is null || !grades.Any())
        {
            return Result.Success<IReadOnlyList<GradeResponseDto>>(new List<GradeResponseDto>());
        }

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

        return Result.Success<IReadOnlyList<GradeResponseDto>>(gradeDtos);
    }

    public async Task<Result<GradeResponseDto?>> GetByIdAsync(Guid Id, CancellationToken ct = default)
    {
        var grade = await _unitOfWork.GradeRepository.GetByIdWithIncludeAsync(
                        Id,
                        ct,
                        p => p.Subjects,
                        p => p.School,
                        p => p.Users
                        );

        if (grade is null)
        {
            return Result.Failure<GradeResponseDto?>(
                new Error("Grade.NotFound", $"Grade with ID '{Id}' was not found."));
        }

        var gradeDto = new GradeResponseDto
        {
            Id = grade.Id,
            Name = grade.Name,
            Description = grade.Description,
            IsActive = grade.IsActive,
            SchoolId = grade.SchoolId,
            SchoolName = grade.School?.Name ?? string.Empty,
        };

        return Result.Success<GradeResponseDto?>(gradeDto);
    }

    public async Task<Result<IReadOnlyList<GradeResponseDto>>> SearchAsync(string gradeName, CancellationToken ct = default)
    {
        var searchTerm = gradeName?.ToLower() ?? string.Empty;

        var grades = await _unitOfWork.GradeRepository.GetAllWithIncludesAsync(
                         g => g.Name.ToLower().Contains(searchTerm),
                         ct,
                         p => p.Subjects,
                         p => p.School,
                         p => p.Users
                         );

        // For searches, returning an empty list on no matches is standard REST practice
        if (grades is null || !grades.Any())
        {
            return Result.Success<IReadOnlyList<GradeResponseDto>>(new List<GradeResponseDto>());
        }

        var gradeDtos = grades.Select(g => new GradeResponseDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            IsActive = g.IsActive,
            SchoolId = g.SchoolId,
            SchoolName = g.School?.Name ?? string.Empty,
        }).ToList();

        return Result.Success<IReadOnlyList<GradeResponseDto>>(gradeDtos);
    }

    public async Task<Result<GradeResponseDto?>> CreateAsync(CreateGradeRequest createRequestDto, CancellationToken ct = default)
    {
        // 1. Validate School exists
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(createRequestDto.SchoolId, ct);
        if (school is null)
        {
            return Result.Failure<GradeResponseDto?>(
                new Error("School.NotFound", $"School with ID '{createRequestDto.SchoolId}' was not found."));
        }

        // 2. Create entity
        var grade = new Grade
        {
            Name = createRequestDto.Name,
            Description = createRequestDto.Description,
            SchoolId = createRequestDto.SchoolId,
            IsActive = true
        };

        await _unitOfWork.GradeRepository.AddAsync(grade, ct);

        // 3. Save changes safely
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return Result.Failure<GradeResponseDto?>(
                new Error("Grade.CreationFailed", $"Failed to create grade. Reason: {ex.InnerException?.Message ?? ex.Message}"));
        }

        // 4. Map and return
        var gradeResponseDto = new GradeResponseDto
        {
            Id = grade.Id,
            Name = grade.Name,
            Description = grade.Description,
            IsActive = grade.IsActive,
            SchoolId = grade.SchoolId,
            SchoolName = school.Name
        };

        return Result.Success<GradeResponseDto?>(gradeResponseDto);
    }

    public async Task<Result<GradeResponseDto?>> UpdateAsync(Guid Id, UpdateGradeRequest updateRequestDto, CancellationToken cancellationToken = default)
    {
        // 1. Fetch the grade AND its School in one trip
        var grade = await _unitOfWork.GradeRepository.GetByIdWithIncludeAsync(
                        Id,
                        cancellationToken,
                        p => p.School
                        );

        if (grade is null)
        {
            return Result.Failure<GradeResponseDto?>(
                new Error("Grade.NotFound", $"Grade with ID '{Id}' was not found."));
        }

        // 2. Update fields
        grade.Name = updateRequestDto.Name;
        grade.Description = updateRequestDto.Description;
        grade.IsActive = updateRequestDto.IsActive;
        grade.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.GradeRepository.Update(grade);

        // 3. Save changes safely
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Failure<GradeResponseDto?>(
                new Error("Grade.UpdateFailed", $"Failed to update grade. Reason: {ex.InnerException?.Message ?? ex.Message}"));
        }

        // 4. Map to DTO
        var gradeResponseDto = new GradeResponseDto
        {
            Id = grade.Id,
            Name = grade.Name,
            Description = grade.Description,
            IsActive = grade.IsActive,
            SchoolId = grade.SchoolId,
            SchoolName = grade.School?.Name ?? string.Empty
        };

        return Result.Success<GradeResponseDto?>(gradeResponseDto);
    }

    public async Task<Result> DeleteAsync(Guid Id, CancellationToken ct = default)
    {
        var grade = await _unitOfWork.GradeRepository.GetByIdAsync(Id, ct);

        if (grade is null)
        {
            return Result.Failure(new Error("Grade.NotFound", $"Grade with ID '{Id}' was not found."));
        }

        _unitOfWork.GradeRepository.Delete(grade);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return Result.Failure(
                new Error("Grade.DeletionFailed", $"Failed to delete grade. Reason: {ex.InnerException?.Message ?? ex.Message}"));
        }

        return Result.Success();
    }
}