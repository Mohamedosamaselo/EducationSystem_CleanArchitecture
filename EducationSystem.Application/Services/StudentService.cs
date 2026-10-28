using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Abstarctions.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StudentGradeResponseDto>> GetMyGradeAsync(
     Guid studentId,
     CancellationToken ct = default)
    {
        var student = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.Id == studentId, ct);

        if (student is null)
        {
            return Result.Failure<StudentGradeResponseDto>(
                new Error("Student.NotFound", "Student was not found."));
        }

        if (student.GradeId is null)
        {
            return Result.Failure<StudentGradeResponseDto>(
                new Error("Student.GradeNotFound", "No grade is assigned to this student."));
        }

        var grade = await _unitOfWork.GradeRepository
            .FirstOrDefaultAsync(g => g.Id == student.GradeId.Value, ct);

        if (grade is null)
        {
            return Result.Failure<StudentGradeResponseDto>(
                new Error("Grade.NotFound", "The student's grade was not found."));
        }

        return Result.Success(new StudentGradeResponseDto(
            grade.Id,
            grade.Name));
    }

    public async Task<Result<StudentEvaluationResponseDto>> GetMySubjectEvaluationAsync(
    Guid studentId,
    Guid subjectId,
    CancellationToken ct = default)
    {
        var userSubject = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                us => us.UserId == studentId &&
                      us.SubjectId == subjectId,
                ct);

        if (userSubject is null)
        {
            return Result.Failure<StudentEvaluationResponseDto>(
                new Error(
                    "Evaluation.NotFound",
                    "You are not enrolled in this subject, or no evaluation record exists."));
        }

        var response = new StudentEvaluationResponseDto(
            userSubject.UserId,
            userSubject.SubjectId,
            userSubject.Subject.Name,
            userSubject.FinalEvaluation);

        return Result.Success(response);
    }

    public async Task<Result<IReadOnlyList<StudentSubjectResponseDto>>> GetMySubjectsAsync(
      Guid studentId,
      CancellationToken ct = default)
    {
        var student = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.Id == studentId, ct);

        if (student is null)
        {
            return Result.Failure<IReadOnlyList<StudentSubjectResponseDto>>(
                new Error("Student.NotFound", "Student was not found."));
        }

        var subjects = new Dictionary<Guid, StudentSubjectResponseDto>();

        // Subjects belonging to the student's grade
        if (student.GradeId.HasValue)
        {
            var grade = await _unitOfWork.GradeRepository
                .FirstOrDefaultAsync(g => g.Id == student.GradeId.Value, ct);

            if (grade is not null)
            {
                foreach (var subject in grade.Subjects)
                {
                    subjects[subject.Id] = new StudentSubjectResponseDto(
                        subject.Id, subject.Name);
                }
            }
        }

        // Subjects in which the student is enrolled
        var enrollments = await _unitOfWork.UserSubjectRepository
            .GetAllAsync(us => us.UserId == studentId, ct);

        foreach (var enrollment in enrollments)
        {
            subjects[enrollment.SubjectId] =
                new StudentSubjectResponseDto(
                    enrollment.SubjectId,
                    enrollment.Subject.Name);
        }

        return Result.Success<IReadOnlyList<StudentSubjectResponseDto>>(
            subjects.Values.ToList());
    }

    public async Task<Result<IReadOnlyList<StudentTeacherResponseDto>>> GetMyTeachersAsync(
      Guid studentId,
      CancellationToken ct = default)
    {
        var student = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.Id == studentId, ct);

        if (student is null)
        {
            return Result.Failure<IReadOnlyList<StudentTeacherResponseDto>>(
                new Error("Student.NotFound", "Student was not found."));
        }

        if (!student.GradeId.HasValue)
        {
            return Result.Success<IReadOnlyList<StudentTeacherResponseDto>>(
                Array.Empty<StudentTeacherResponseDto>());
        }

        var teacherAssignments = await _unitOfWork.TeacherGradeRepository
            .GetAllAsync(tg => tg.GradeId == student.GradeId.Value, ct);

        var teachers = teacherAssignments
            .Where(tg => tg.Teacher is not null)
            .Select(tg => new StudentTeacherResponseDto(
                tg.Teacher.Id,
                tg.Teacher.FirstName,
                tg.Teacher.LastName))
            .DistinctBy(t => t.Id)
            .ToList();

        return Result.Success<IReadOnlyList<StudentTeacherResponseDto>>(teachers);
    }

    public async Task<Result<IReadOnlyList<StudentEvaluationResponseDto>>> GetMyEvaluationsAsync(
    Guid studentId,
    CancellationToken ct = default)
    {
        var student = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(u => u.Id == studentId, ct);

        if (student is null)
        {
            return Result.Failure<IReadOnlyList<StudentEvaluationResponseDto>>(
                new Error("Student.NotFound", "Student was not found."));
        }

        var userSubjects = await _unitOfWork.UserSubjectRepository
            .GetAllAsync(us => us.UserId == studentId, ct);

        var evaluations = userSubjects
            .Select(us => new StudentEvaluationResponseDto(
                us.UserId,
                us.SubjectId,
                us.Subject.Name,
                us.FinalEvaluation))
            .ToList();

        return Result.Success<IReadOnlyList<StudentEvaluationResponseDto>>(
            evaluations);
    }
}