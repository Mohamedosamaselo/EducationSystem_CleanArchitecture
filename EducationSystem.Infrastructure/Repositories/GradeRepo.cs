using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Infrastructure.Repositories;

public class GradeRepo(ApplicationDbContext context)
                     : GenericRepo<Grade>(context), IGradeRepo
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> SchoolHasSubjectAsync(
        Guid schoolId,
        Guid subjectId,
        CancellationToken ct = default)
    {
        return await _context.Grades.AnyAsync(
            g => g.SchoolId == schoolId &&
                 g.Subjects.Any(s => s.Id == subjectId),
            ct);
    }

    public async Task<bool> GradeHasSubjectAsync(
        Guid gradeId,
        Guid schoolId,
        Guid subjectId,
        CancellationToken ct = default)
    {
        return await _context.Grades.AnyAsync(
            g => g.Id == gradeId &&
                 g.SchoolId == schoolId &&
                 g.Subjects.Any(s => s.Id == subjectId),
            ct);
    }

    public async Task<IReadOnlyList<Grade>> GetTeacherGradesAsync(
        Guid teacherId,
        Guid schoolId,
        CancellationToken ct = default)
    {
        return await _context.TeacherGrades
            .Where(tg =>
                tg.TeacherId == teacherId &&
                tg.Grade.SchoolId == schoolId)
            .Select(tg => tg.Grade)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}