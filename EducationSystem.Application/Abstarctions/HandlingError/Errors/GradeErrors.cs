using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Abstarctions.HandlingError.Errors;

public static class GradeErrors
{
    public static readonly Error InvalidInput = new("Grade.InvalidInput", "Invalid input.");
    public static readonly Error NotFound = new("Grade.NotFound", "Grade was not found.");
    public static readonly Error DuplicateName = new("Grade.DuplicateName", "A grade with this name already exists.");
    public static readonly Error HasStudents = new("Grade.HasStudents", "Cannot delete a grade that still has students.");
    public static readonly Error CreationFailed = new("Grade.CreationFailed", "Failed to create the grade.");
    public static readonly Error UpdateFailed = new("Grade.UpdateFailed", "Failed to update the grade.");
    public static readonly Error DeletionFailed = new("Grade.DeletionFailed", "Failed to delete the grade.");
}