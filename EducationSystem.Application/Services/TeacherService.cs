using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Teacher;
using EducationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class TeacherService(IUnitOfWork unitOfWork) : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private async Task<bool> IsAssignedToGradeAsync(
        Guid teacherId,
        Guid gradeId,
        CancellationToken ct)
    {
        var assignment = await _unitOfWork.TeacherGradeRepository
            .FirstOrDefaultAsync(
                x => x.TeacherId == teacherId && x.GradeId == gradeId,
                ct);

        return assignment is not null;
    }

    private async Task<Grade?> GetGradeAsync(
        Guid gradeId,
        CancellationToken ct)
    {
        return await _unitOfWork.GradeRepository
            .FirstOrDefaultAsync(x => x.Id == gradeId, ct);
    }

    private async Task<ApplicationUser?> GetStudentAsync(
        Guid studentId,
        CancellationToken ct)
    {
        return await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(x => x.Id == studentId, ct);
    }

    private async Task<Subject?> GetSubjectAsync(
        Guid subjectId,
        CancellationToken ct)
    {
        return await _unitOfWork.SubjectRepository
            .FirstOrDefaultAsync(x => x.Id == subjectId, ct);
    }

    public async Task<Result<IReadOnlyList<TeacherGradeResponseDto>>> GetMyGradesAsync(
        Guid teacherId,
        CancellationToken ct)
    {
        var assignments = await _unitOfWork.TeacherGradeRepository
            .GetAllAsync(x => x.TeacherId == teacherId, ct);

        var grades = new List<TeacherGradeResponseDto>();

        foreach (var assignment in assignments)
        {
            var grade = await GetGradeAsync(assignment.GradeId, ct);

            if (grade is null)
                continue;

            grades.Add(new TeacherGradeResponseDto(
                grade.Id,
                grade.Name,
                grade.Description));
        }

        return Result.Success<IReadOnlyList<TeacherGradeResponseDto>>(grades);
    }

    public async Task<Result<IReadOnlyList<TeacherSubjectResponseDto>>> GetMySubjectsAsync(
        Guid teacherId,
        CancellationToken ct)
    {
        var assignments = await _unitOfWork.TeacherGradeRepository
            .GetAllAsync(x => x.TeacherId == teacherId, ct);

        var subjectIds = new HashSet<Guid>();
        var subjects = new List<TeacherSubjectResponseDto>();

        foreach (var assignment in assignments)
        {
            var grade = await _unitOfWork.GradeRepository
                .FirstOrDefaultAsync(
                    x => x.Id == assignment.GradeId,
                    ct);

            if (grade is null)
                continue;

            // Grade.Subjects must be loaded by your repository/query.
            foreach (var subject in grade.Subjects)
            {
                if (subjectIds.Add(subject.Id))
                {
                    subjects.Add(new TeacherSubjectResponseDto(
                        subject.Id,
                        subject.Name));
                }
            }
        }

        return Result.Success<IReadOnlyList<TeacherSubjectResponseDto>>(subjects);
    }

    public async Task<Result<IReadOnlyList<TeacherStudentResponseDto>>> GetGradeStudentsAsync(
        Guid teacherId,
        Guid gradeId,
        CancellationToken ct)
    {
        if (!await IsAssignedToGradeAsync(teacherId, gradeId, ct))
        {
            return Result.Failure<IReadOnlyList<TeacherStudentResponseDto>>(
                new Error("Teacher.GradeNotAssigned",
                    "You are not assigned to this grade."));
        }

        var students = await _unitOfWork.UserRepository
            .GetAllAsync(x => x.GradeId == gradeId, ct);

        var response = students.Select(MapStudent).ToList();

        return Result.Success<IReadOnlyList<TeacherStudentResponseDto>>(response);
    }

    public async Task<Result<IReadOnlyList<TeacherStudentResponseDto>>> GetSubjectStudentsAsync(
        Guid teacherId,
        Guid gradeId,
        Guid subjectId,
        CancellationToken ct)
    {
        if (!await IsAssignedToGradeAsync(teacherId, gradeId, ct))
        {
            return Result.Failure<IReadOnlyList<TeacherStudentResponseDto>>(
                new Error("Teacher.GradeNotAssigned",
                    "You are not assigned to this grade."));
        }

        var grade = await GetGradeAsync(gradeId, ct);

        if (grade is null)
        {
            return Result.Failure<IReadOnlyList<TeacherStudentResponseDto>>(
                new Error("Grade.NotFound", "Grade was not found."));
        }

        var subject = await GetSubjectAsync(subjectId, ct);

        if (subject is null || !grade.Subjects.Any(s => s.Id == subjectId))
        {
            return Result.Failure<IReadOnlyList<TeacherStudentResponseDto>>(
                new Error("Subject.NotInGrade",
                    "This subject is not assigned to the requested grade."));
        }

        var students = await _unitOfWork.UserRepository
            .GetAllAsync(x => x.GradeId == gradeId, ct);

        return Result.Success<IReadOnlyList<TeacherStudentResponseDto>>(
            students.Select(MapStudent).ToList());
    }

    public async Task<Result<TeacherStudentResponseDto>> GetStudentAsync(
        Guid teacherId,
        Guid studentId,
        CancellationToken ct)
    {
        var student = await GetStudentAsync(studentId, ct);

        if (student is null)
        {
            return Result.Failure<TeacherStudentResponseDto>(
                new Error("Student.NotFound", "Student was not found."));
        }

        if (student.GradeId is null ||
            !await IsAssignedToGradeAsync(teacherId, student.GradeId.Value, ct))
        {
            return Result.Failure<TeacherStudentResponseDto>(
                new Error("Teacher.StudentNotAssigned",
                    "This student does not belong to any of your assigned grades."));
        }

        return Result.Success(MapStudent(student));
    }

    public async Task<Result<IReadOnlyList<TeacherEvaluationResponseDto>>> GetEvaluationsAsync(
        Guid teacherId,
        Guid subjectId,
        Guid gradeId,
        CancellationToken ct)
    {
        if (!await IsAssignedToGradeAsync(teacherId, gradeId, ct))
        {
            return Result.Failure<IReadOnlyList<TeacherEvaluationResponseDto>>(
                new Error("Teacher.GradeNotAssigned",
                    "You are not assigned to this grade."));
        }

        var grade = await GetGradeAsync(gradeId, ct);

        if (grade is null || !grade.Subjects.Any(s => s.Id == subjectId))
        {
            return Result.Failure<IReadOnlyList<TeacherEvaluationResponseDto>>(
                new Error("Subject.NotInGrade",
                    "This subject is not assigned to the requested grade."));
        }

        var records = await _unitOfWork.UserSubjectRepository
            .GetAllAsync(x =>
                x.SubjectId == subjectId &&
                x.User.GradeId == gradeId, ct);

        var response = records.Select(x => new TeacherEvaluationResponseDto(
            x.UserId,
            $"{x.User.FirstName} {x.User.LastName}",
            x.SubjectId,
            x.Subject.Name,
            x.FinalEvaluation)).ToList();

        return Result.Success<IReadOnlyList<TeacherEvaluationResponseDto>>(response);
    }

    public async Task<Result> AddEvaluationAsync(
        Guid teacherId,
        Guid subjectId,
        Guid studentId,
        string finalEvaluation,
        CancellationToken ct)
    {
        var validation = await ValidateEvaluationAccessAsync(teacherId, subjectId, studentId, ct);

        if (validation is not null)
            return Result.Failure(validation);

        var existing = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                x => x.UserId == studentId && x.SubjectId == subjectId,
                ct);

        if (existing is null)
        {
            return Result.Failure(
                new Error("StudentSubject.NotFound",
                    "The student is not enrolled in this subject."));
        }

        if (!string.IsNullOrWhiteSpace(existing.FinalEvaluation))
        {
            return Result.Failure(
                new Error("Evaluation.AlreadyExists",
                    "An evaluation already exists for this student and subject."));
        }

        existing.FinalEvaluation = finalEvaluation;
        _unitOfWork.UserSubjectRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> UpdateEvaluationAsync(
        Guid teacherId,
        Guid subjectId,
        Guid studentId,
        string finalEvaluation,
        CancellationToken ct)
    {
        var validation = await ValidateEvaluationAccessAsync(
            teacherId, subjectId, studentId, ct);

        if (validation is not null)
            return Result.Failure(validation);

        var existing = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                x => x.UserId == studentId && x.SubjectId == subjectId,
                ct);

        if (existing is null ||
            string.IsNullOrWhiteSpace(existing.FinalEvaluation))
        {
            return Result.Failure(
                new Error("Evaluation.NotFound",
                    "No evaluation exists for this student and subject."));
        }

        existing.FinalEvaluation = finalEvaluation;
        _unitOfWork.UserSubjectRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> DeleteEvaluationAsync(
        Guid teacherId,
        Guid subjectId,
        Guid studentId,
        CancellationToken ct)
    {
        var validation = await ValidateEvaluationAccessAsync(
            teacherId, subjectId, studentId, ct);

        if (validation is not null)
            return Result.Failure(validation);

        var existing = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                x => x.UserId == studentId && x.SubjectId == subjectId,
                ct);

        if (existing is null ||
            string.IsNullOrWhiteSpace(existing.FinalEvaluation))
        {
            return Result.Failure(
                new Error("Evaluation.NotFound",
                    "No evaluation exists for this student and subject."));
        }

        // Keep the student's subject enrollment; clear only the evaluation.
        existing.FinalEvaluation = null;
        _unitOfWork.UserSubjectRepository.Update(existing);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    private async Task<Error?> ValidateEvaluationAccessAsync(Guid teacherId,
                                                                Guid subjectId,
                                                                Guid studentId,
                                                                CancellationToken ct)
    {
        var student = await GetStudentAsync(studentId, ct);

        if (student is null || student.GradeId is null)
            return new Error("Student.NotFound", "Student or student's grade was not found.");

        if (!await IsAssignedToGradeAsync(teacherId, student.GradeId.Value, ct))
            return new Error("Teacher.StudentNotAssigned",
                "You are not assigned to this student's grade.");

        var grade = await GetGradeAsync(student.GradeId.Value, ct);

        if (grade is null || !grade.Subjects.Any(s => s.Id == subjectId))
            return new Error("Subject.NotInGrade",
                "This subject is not assigned to the student's grade.");

        return null;
    }

    private static TeacherStudentResponseDto MapStudent(ApplicationUser student)
    {
        return new TeacherStudentResponseDto(
            student.Id,
            student.FirstName,
            student.LastName,
            student.Email ?? string.Empty);
    }
}