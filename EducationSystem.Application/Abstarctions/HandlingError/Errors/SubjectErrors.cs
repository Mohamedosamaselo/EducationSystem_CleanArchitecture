using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Abstarctions.HandlingError.Errors;

public static class SubjectErrors
{
    public static readonly Error InvalidInput = new("Subject.InvalidInput", "Invalid input.");
    public static readonly Error NotFound = new("Subject.NotFound", "Subject was not found.");
    public static readonly Error DuplicateName = new("Subject.DuplicateName", "A subject with this name already exists.");
    public static readonly Error AssignedToGrade = new("Subject.AssignedToGrade", "Remove the subject from its grades before deleting it.");
    public static readonly Error CreationFailed = new("Subject.CreationFailed", "Failed to create the subject.");
    public static readonly Error UpdateFailed = new("Subject.UpdateFailed", "Failed to update the subject.");
    public static readonly Error DeletionFailed = new("Subject.DeletionFailed", "Failed to delete the subject.");
}