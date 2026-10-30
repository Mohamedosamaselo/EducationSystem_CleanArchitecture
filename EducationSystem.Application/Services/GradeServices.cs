using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.HandlingError.Errors;
using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Application.Dtos.Subject;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Services;

public class GradeServices(IUnitOfWork unitOfWork, IGradeRepo gradeRepository) : IGradeService

{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGradeRepo gradeRepository = gradeRepository;

    public async Task<Result<IReadOnlyList<GradeResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        // load navigational properties [Subject , school , users ] eager with grades
        var grades = await gradeRepository.GetAllWithIncludesAsync(null, ct,
                         p => p.Subjects,
                         p => p.School
                         //p => p.Users
                            );

        // If there are no grades, return a successful Result with an empty list.
        if (grades is null || !grades.Any())
        {
            return Result.Success<IReadOnlyList<GradeResponseDto>>(new List<GradeResponseDto>());
        }

        var gradeDtos = grades
            .Select(g => new GradeResponseDto(
                g.Id,
                g.Name,
                g.Description,
                g.SchoolId,
                g.School.Name))
            .ToList();

        return Result.Success<IReadOnlyList<GradeResponseDto>>(gradeDtos);
    }

    public async Task<Result<GradeResponseDto?>> GetByIdAsync(Guid Id, CancellationToken ct = default)
    {
        var grade = await gradeRepository.GetByIdWithIncludeAsync(Id,
                                                                                ct,
                                                                                p => p.Subjects,
                                                                                p => p.School
                                                                                );

        if (grade is null)
            return Result.Failure<GradeResponseDto?>(
                new Error("Grade.NotFound", $"Grade with ID '{Id}' was not found."));

        var gradeDto = new GradeResponseDto(
                                             grade.Id,
                                             grade.Name,
                                             grade.Description,
                                             grade.SchoolId,
                                             grade.School?.Name ?? string.Empty
                                                        );
        return Result.Success<GradeResponseDto?>(gradeDto);
    }

    //public async Task<Result<IReadOnlyList<GradeResponseDto>>> SearchAsync(string gradeName, CancellationToken ct = default)
    //{
    //    var searchTerm = gradeName?.ToLower() ?? string.Empty;

    //    var grades = await _unitOfWork.GradeRepository.GetAllWithIncludesAsync(
    //                     g => g.Name.ToLower().Contains(searchTerm),
    //                     ct,
    //                     p => p.Subjects,
    //                     p => p.School
    //                     //p => p.Users
    //                     );

    //    // For searches, returning an empty list on no matches is standard REST practice
    //    if (grades is null || !grades.Any())
    //    {
    //        return Result.Success<IReadOnlyList<GradeResponseDto>>(new List<GradeResponseDto>());
    //    }

    //    var gradeDtos = grades.Select(g => new GradeResponseDto
    //    {
    //        Id = g.Id,
    //        Name = g.Name,
    //        Description = g.Description,
    //        //IsActive = g.IsActive,
    //        SchoolId = g.SchoolId,
    //        SchoolName = g.School?.Name ?? string.Empty,
    //    }).ToList();

    //    return Result.Success<IReadOnlyList<GradeResponseDto>>(gradeDtos);
    //}

    public async Task<Result<GradeResponseDto?>> AddAsync(CreateGradeRequest dto, CancellationToken ct = default)
    {
        // 1. Validate input
        if (dto is null || string.IsNullOrWhiteSpace(dto.Name))
            return Result.Failure<GradeResponseDto?>(GradeErrors.InvalidInput);

        var name = dto.Name.Trim();

        // Check for duplicate name (case-insensitive) across all grades
        var duplicate = await gradeRepository
                   .FirstOrDefaultAsync(g => g.Name == name, ct);
        if (duplicate is not null)
            return Result.Failure<GradeResponseDto?>(GradeErrors.DuplicateName);

        // Validate School exists
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(dto.SchoolId, ct);
        if (school is null)
            return Result.Failure<GradeResponseDto?>(new Error("School.NotFound", $"School with ID '{dto.SchoolId}' was not found."));

        // 2. Create entity
        var grade = new Grade
        {
            Name = dto.Name,
            Description = dto.Description,
            SchoolId = dto.SchoolId,
        };

        await gradeRepository.AddAsync(grade, ct);

        // 3. Save changes safely
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return Result.Failure<GradeResponseDto?>(new Error("Grade.CreationFailed", $"Failed to create grade. Reason: {ex.InnerException?.Message ?? ex.Message}"));
        }

        // 4. Map and return
        var gradeResponseDto = new GradeResponseDto(
                                    grade.Id,
                                    grade.Name,
                                    grade.Description,
                                    grade.SchoolId,
                                    grade.School?.Name ?? string.Empty
                                );

        return Result.Success<GradeResponseDto?>(gradeResponseDto);
    }

    public async Task<Result<GradeResponseDto?>> UpdateAsync(Guid id, UpdateGradeRequest dto, CancellationToken ct = default)
    {
        // 1. Validate input
        if (id == Guid.Empty || dto is null || string.IsNullOrWhiteSpace(dto.Name))
            return Result.Failure<GradeResponseDto?>(GradeErrors.InvalidInput);

        // 2. Check if the grade exists
        var grade = await gradeRepository.GetByIdAsync(id, ct);
        if (grade is null)
            return Result.Failure<GradeResponseDto?>(GradeErrors.NotFound);

        var name = dto.Name.Trim();
        // 3. Check for duplicate name (case-insensitive) across all grades except the one being updated
        var duplicate = await gradeRepository.FirstOrDefaultAsync(g => g.Name == name && g.Id != id, ct);

        if (duplicate is not null)
            return Result.Failure<GradeResponseDto?>(GradeErrors.DuplicateName);

        // 2. Update the properties
        grade.Name = dto.Name;
        grade.Description = dto.Description;
        grade.ModifiedAt = DateTime.UtcNow;

        await gradeRepository.UpdateAsync(grade, ct);

        // 3. Save changes safely
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return Result.Failure<GradeResponseDto?>(new Error("Grade.UpdateFailed", $"Failed to update grade. Reason: {ex.InnerException?.Message ?? ex.Message}"));
        }

        // 4. Map to DTO
        var gradeResponseDto = new GradeResponseDto(
                                            grade.Id,
                                            grade.Name,
                                            grade.Description,
                                            grade.SchoolId,
                                            grade.School?.Name ?? string.Empty
                                        );

        return Result.Success<GradeResponseDto?>(gradeResponseDto);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        // 1. Validate input
        if (id == Guid.Empty)
            return Result.Failure(GradeErrors.InvalidInput);
        // 2. Check if the grade exists
        var grade = await gradeRepository.GetByIdAsync(id, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        // Do not orphan students
        var hasStudents = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.GradeId == id, ct);
        if (hasStudents is not null)
            return Result.Failure(GradeErrors.HasStudents);

        await gradeRepository.DeleteAsync(grade, ct);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return Result.Failure(error: new Error("Grade.DeletionFailed", $"Failed to delete grade. Reason: {ex.InnerException?.Message ?? ex.Message}"));
        }

        return Result.Success();
    }

    // GET /grades/{gradeId}/subjects
    public async Task<Result<IReadOnlyList<SubjectResponse>>> GetSubjectsAsync(Guid gradeId, CancellationToken ct = default)
    {
        if (gradeId == Guid.Empty)
            return Result.Failure<IReadOnlyList<SubjectResponse>>(GradeErrors.InvalidInput);

        var grade = await _unitOfWork.GradeRepository
            .GetByIdWithIncludeAsync(gradeId, ct, g => g.Subjects);
        if (grade is null)
            return Result.Failure<IReadOnlyList<SubjectResponse>>(GradeErrors.NotFound);

        var response = grade.Subjects
            .Select(s => new SubjectResponse { Id = s.Id, Name = s.Name })
            .ToList();

        return Result.Success<IReadOnlyList<SubjectResponse>>(response);
    }

    // Assign a subject to a grade (inserts a row in the GradeSubject join table)
    public async Task<Result<GradeSubjectResponse?>> AssignSubjectAsync(
        Guid gradeId, AssignSubjectToGradeRequest request, CancellationToken ct = default)
    {
        if (gradeId == Guid.Empty || request is null || request.SubjectId == Guid.Empty)
            return Result.Failure<GradeSubjectResponse?>(GradeSubjectErrors.InvalidInput);

        // Load the grade WITH its subjects
        var grade = await _unitOfWork.GradeRepository
            .GetByIdWithIncludeAsync(gradeId, ct, g => g.Subjects);

        if (grade is null)
            return Result.Failure<GradeSubjectResponse?>(GradeSubjectErrors.GradeNotFound);

        var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(request.SubjectId, ct);
        if (subject is null)
            return Result.Failure<GradeSubjectResponse?>(GradeSubjectErrors.SubjectNotFound);

        // Already assigned?
        if (grade.Subjects.Any(s => s.Id == subject.Id))
            return Result.Failure<GradeSubjectResponse?>(GradeSubjectErrors.AlreadyExists);

        // Adding to the collection creates the join row on save
        grade.Subjects.Add(subject);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            return Result.Failure<GradeSubjectResponse?>(GradeSubjectErrors.CreationFailed);
        }

        return Result.Success<GradeSubjectResponse?>(new GradeSubjectResponse
        {
            GradeId = grade.Id,
            GradeName = grade.Name,
            SubjectId = subject.Id,
            SubjectName = subject.Name
        });
    }

    // Remove a subject from a grade (deletes the join row only)
    public async Task<Result> RemoveSubjectAsync(
        Guid gradeId, Guid subjectId, CancellationToken ct = default)
    {
        if (gradeId == Guid.Empty || subjectId == Guid.Empty)
            return Result.Failure(GradeSubjectErrors.InvalidInput);

        var grade = await _unitOfWork.GradeRepository
            .GetByIdWithIncludeTrackedAsync(gradeId, ct, g => g.Subjects);
        if (grade is null)
            return Result.Failure(GradeSubjectErrors.GradeNotFound);

        var subject = grade.Subjects.FirstOrDefault(s => s.Id == subjectId);
        if (subject is null)
            return Result.Failure(GradeSubjectErrors.NotFound);

        // Removing from the collection deletes the join row, not the subject itself
        var removingResult = grade.Subjects.Remove(subject);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            return Result.Failure(GradeSubjectErrors.DeletionFailed);
        }

        return Result.Success();
    }
}