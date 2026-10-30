using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class SchoolAssignmentService(
    IUnitOfWork unitOfWork,
    IGradeRepo gradeRepository,
    RoleManager<ApplicationRole> roleManager) : ISchoolAssignmentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGradeRepo _gradeRepository = gradeRepository;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<IReadOnlyList<GradeResponseDto>>> GetTeacherGradesAsync(
      Guid schoolAdminId,
      Guid teacherId,
      CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
        {
            return Result.Failure<IReadOnlyList<GradeResponseDto>>(
                new Error("School.NotFound", "Your assigned school was not found."));
        }

        var teacher = await GetSchoolUserAsync(
            school.Id, teacherId, "Teacher", ct);

        if (teacher is null)
        {
            return Result.Failure<IReadOnlyList<GradeResponseDto>>(
                new Error("Teacher.NotFound", "Teacher was not found in your school."));
        }

        var grades = await _gradeRepository
            .GetTeacherGradesAsync(teacherId, school.Id, ct);

        var response = grades
            .Select(g => new GradeResponseDto(
                g.Id,
                g.Name,
                g.Description,
                g.SchoolId,
                g.School.Name))
            .ToList();

        return Result.Success<IReadOnlyList<GradeResponseDto>>(response);
    }

    public async Task<Result<bool>> AssignTeacherToGradeAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid gradeId,
        CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<bool>(
                new Error("School.NotFound", "Your assigned school was not found."));

        var teacher = await GetSchoolUserAsync(
            school.Id, teacherId, "Teacher", ct);

        if (teacher is null)
            return Result.Failure<bool>(
                new Error("Teacher.NotFound", "Teacher was not found in your school."));

        var grade = await _unitOfWork.GradeRepository.FirstOrDefaultAsync(
            g => g.Id == gradeId && g.SchoolId == school.Id, ct);

        if (grade is null)
            return Result.Failure<bool>(
                new Error("Grade.NotFound", "Grade was not found in your school."));

        var existing = await _unitOfWork.TeacherGradeRepository
            .FirstOrDefaultAsync(
                x => x.TeacherId == teacherId && x.GradeId == gradeId, ct);

        if (existing is not null)
            return Result.Failure<bool>(
                new Error("TeacherGrade.AlreadyAssigned",
                    "This teacher is already assigned to this grade."));

        await _unitOfWork.TeacherGradeRepository.AddAsync(
            new TeacherGrade
            {
                TeacherId = teacherId,
                GradeId = gradeId
            }, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }

    public async Task<Result<bool>> RemoveTeacherFromGradeAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid gradeId,
        CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<bool>(
                new Error("School.NotFound", "Your assigned school was not found."));

        var teacher = await GetSchoolUserAsync(
            school.Id, teacherId, "Teacher", ct);

        if (teacher is null)
            return Result.Failure<bool>(
                new Error("Teacher.NotFound", "Teacher was not found in your school."));

        var grade = await _unitOfWork.GradeRepository.FirstOrDefaultAsync(
            g => g.Id == gradeId && g.SchoolId == school.Id, ct);

        if (grade is null)
            return Result.Failure<bool>(
                new Error("Grade.NotFound", "Grade was not found in your school."));

        var assignment = await _unitOfWork.TeacherGradeRepository
            .FirstOrDefaultAsync(
                x => x.TeacherId == teacherId && x.GradeId == gradeId, ct);

        if (assignment is null)
            return Result.Failure<bool>(
                new Error("TeacherGrade.NotFound",
                    "This teacher is not assigned to this grade."));

        _unitOfWork.TeacherGradeRepository.Delete(assignment);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }

    public async Task<Result<bool>> AssignTeacherToSubjectAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid subjectId,
        CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<bool>(
                new Error("School.NotFound", "Your assigned school was not found."));

        var teacher = await GetSchoolUserAsync(
            school.Id, teacherId, "Teacher", ct);

        if (teacher is null)
            return Result.Failure<bool>(
                new Error("Teacher.NotFound", "Teacher was not found in your school."));

        var subject = await _unitOfWork.SubjectRepository
            .FirstOrDefaultAsync(s => s.Id == subjectId, ct);

        if (subject is null)
            return Result.Failure<bool>(
                new Error("Subject.NotFound", "Subject was not found."));

        // Validate that this subject is offered by at least one
        // grade in this school before assigning it to the teacher.
        var subjectIsOffered = await _gradeRepository
            .SchoolHasSubjectAsync(school.Id, subjectId, ct);

        if (!subjectIsOffered)
            return Result.Failure<bool>(
                new Error("Subject.NotAvailableInSchool",
                    "This subject is not assigned to any grade in your school."));

        var existing = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                x => x.UserId == teacherId && x.SubjectId == subjectId, ct);

        if (existing is not null)
            return Result.Failure<bool>(
                new Error("UserSubject.AlreadyAssigned",
                    "This teacher is already assigned to this subject."));

        await _unitOfWork.UserSubjectRepository.AddAsync(
            new UserSubject
            {
                UserId = teacherId,
                SubjectId = subjectId
            }, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }

    public async Task<Result<bool>> RemoveTeacherFromSubjectAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid subjectId,
        CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<bool>(
                new Error("School.NotFound", "Your assigned school was not found."));

        var teacher = await GetSchoolUserAsync(
            school.Id, teacherId, "Teacher", ct);

        if (teacher is null)
            return Result.Failure<bool>(
                new Error("Teacher.NotFound", "Teacher was not found in your school."));

        var assignment = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                x => x.UserId == teacherId && x.SubjectId == subjectId, ct);

        if (assignment is null)
            return Result.Failure<bool>(
                new Error("UserSubject.NotFound",
                    "This teacher is not assigned to this subject."));

        _unitOfWork.UserSubjectRepository.Delete(assignment);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }

    public async Task<Result<bool>> EnrolStudentInSubjectAsync(
        Guid schoolAdminId,
        Guid studentId,
        Guid subjectId,
        CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<bool>(
                new Error("School.NotFound", "Your assigned school was not found."));

        var student = await GetSchoolUserAsync(
            school.Id, studentId, "Student", ct);

        if (student is null)
            return Result.Failure<bool>(
                new Error("Student.NotFound", "Student was not found in your school."));

        var subject = await _unitOfWork.SubjectRepository
            .FirstOrDefaultAsync(s => s.Id == subjectId, ct);

        if (subject is null)
            return Result.Failure<bool>(
                new Error("Subject.NotFound", "Subject was not found."));

        // Assumes students have a GradeId and a grade may offer subjects.
        if (student.GradeId is null)
            return Result.Failure<bool>(
                new Error("Student.GradeNotAssigned",
                    "The student must be assigned to a grade first."));

        var gradeOffersSubject = await _gradeRepository
            .GradeHasSubjectAsync(student.GradeId.Value, school.Id, subjectId, ct);

        if (!gradeOffersSubject)
            return Result.Failure<bool>(
                new Error("Subject.NotAvailableForStudent",
                    "This subject is not assigned to the student's grade."));

        var existing = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                x => x.UserId == studentId && x.SubjectId == subjectId, ct);

        if (existing is not null)
            return Result.Failure<bool>(
                new Error("UserSubject.AlreadyEnrolled",
                    "The student is already enrolled in this subject."));

        await _unitOfWork.UserSubjectRepository.AddAsync(
            new UserSubject
            {
                UserId = studentId,
                SubjectId = subjectId
            }, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }

    public async Task<Result<bool>> RemoveStudentFromSubjectAsync(
        Guid schoolAdminId,
        Guid studentId,
        Guid subjectId,
        CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<bool>(
                new Error("School.NotFound", "Your assigned school was not found."));

        var student = await GetSchoolUserAsync(
            school.Id, studentId, "Student", ct);

        if (student is null)
            return Result.Failure<bool>(
                new Error("Student.NotFound", "Student was not found in your school."));

        var enrollment = await _unitOfWork.UserSubjectRepository
            .FirstOrDefaultAsync(
                x => x.UserId == studentId && x.SubjectId == subjectId, ct);

        if (enrollment is null)
            return Result.Failure<bool>(
                new Error("UserSubject.NotFound",
                    "The student is not enrolled in this subject."));

        _unitOfWork.UserSubjectRepository.Delete(enrollment);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }

    private async Task<School?> GetAdminSchoolAsync(
        Guid schoolAdminId,
        CancellationToken ct)
    {
        var school = await _unitOfWork.SchoolRepository
            .FirstOrDefaultAsync(
                s => s.SchoolAdminId == schoolAdminId, ct);

        return school;
    }

    private async Task<ApplicationUser?> GetSchoolUserAsync(
       Guid schoolId,
       Guid userId,
       string requiredRole,
       CancellationToken ct)
    {
        var user = await _unitOfWork.UserRepository
            .FirstOrDefaultAsync(
                u => u.Id == userId && u.SchoolId == schoolId, ct);

        if (user is null)
            return null;

        // Resolve the role separately if the Role navigation
        // is not automatically loaded by your repository.
        var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());

        if (role is null || role.Name != requiredRole)
            return null;

        return user;
    }
}