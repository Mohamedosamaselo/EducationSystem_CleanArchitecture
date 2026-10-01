using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Domain.Entities;

public class StudentParent
{
    public Guid StudentId { get; set; }
    public virtual ApplicationUser? Student { get; set; }

    public Guid ParentId { get; set; }
    public virtual ApplicationUser? Parent { get; set; }
}