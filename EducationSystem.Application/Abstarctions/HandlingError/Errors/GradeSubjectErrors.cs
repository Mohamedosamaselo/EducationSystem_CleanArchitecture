namespace EducationSystem.Application.Abstarctions.HandlingError.Errors;

public static class GradeSubjectErrors
{
    public static readonly Error InvalidInput =
        new("GradeSubject.InvalidInput", "Invalid input.");

    public static readonly Error GradeNotFound =
        new("Grade.NotFound", "Grade was not found.");

    public static readonly Error SubjectNotFound =
        new("Subject.NotFound", "Subject was not found.");

    public static readonly Error NotFound =
        new("GradeSubject.NotFound", "This subject is not assigned to this grade.");

    public static readonly Error AlreadyExists =
        new("GradeSubject.AlreadyExists", "This subject is already assigned to this grade.");

    public static readonly Error CreationFailed =
        new("GradeSubject.CreationFailed", "Failed to assign the subject to the grade.");

    public static readonly Error DeletionFailed =
        new("GradeSubject.DeletionFailed", "Failed to remove the subject from the grade.");
}