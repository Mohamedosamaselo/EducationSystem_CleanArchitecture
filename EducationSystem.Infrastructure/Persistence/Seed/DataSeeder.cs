using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        await SeedOrganizationAsync(context);

        await SeedSchoolAsync(context);

        await SeedGradeAsync(context);

        //await SeedSubjectsAsync(context);

        //await SeedRolesAsync(roleManager);

        //await SeedPermissionsAsync(context);

        //await SeedUsersAsync(userManager, context);
    }

    private static async Task SeedOrganizationAsync(ApplicationDbContext context)
    {
        if (await context.Organisations.AnyAsync())
            return;

        var organizations = new List<Organisation>
        {
            new Organisation
            {
                Id = Guid.NewGuid(),
                Name = "Ministery of Education " ,
                Email="MinisteryOfEducation@gmail.com",
                Phone="1552",
                CreatedAt = DateTime.UtcNow,
            }
        };

        await context.AddRangeAsync(organizations);

        await context.SaveChangesAsync();
    }

    private static async Task SeedSchoolAsync(ApplicationDbContext context)
    {
        // Check if there are any schools in the database
        if (await context.Schools.AnyAsync())
            return;

        var organisation = await context.Organisations.FirstOrDefaultAsync();
        if (organisation == null)
            throw new InvalidOperationException("Organisation not found. Please seed the organisation first.");

        var schools = new List<School>
    {
     new School
        {
            Name = "Cairo International School",
            Address = "123 Nile St, Cairo, 11111, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "Giza Modern School",
            Address = "456 Pyramid Ave, Giza, 12222, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "Alexandria Future School",
            Address = "15 Corniche Road, Alexandria, 21500, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "New Cairo International Academy",
            Address = "25 Teseen Street, New Cairo, 11835, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "October Modern Academy",
            Address = "10 Central Avenue, 6th of October, 12566, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "Mansoura Excellence School",
            Address = "78 El Gomhoria Street, Mansoura, 35511, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "Aswan Nile Valley School",
            Address = "32 Corniche El Nile, Aswan, 81511, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "Luxor International Academy",
            Address = "21 Karnak Road, Luxor, 85951, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "Port Said Advanced School",
            Address = "55 El Nasr Street, Port Said, 42511, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        },

        new School
        {
            Name = "Tanta Modern Education School",
            Address = "40 Saad Zaghloul Street, Tanta, 31511, Egypt",
            OrganisationId = organisation.Id,
            CreatedAt = DateTime.UtcNow
        }
    };

        await context.Schools.AddRangeAsync(schools);

        await context.SaveChangesAsync();
    }

    private static async Task SeedGradeAsync(ApplicationDbContext context)
    {
        if (await context.Grades.AnyAsync())
            return;

        var school = await context.Schools.FirstOrDefaultAsync();

        if (school == null)
            throw new InvalidOperationException
                ("School not found. Please seed the school first.");

        var grades = new List<Grade>
       {
        new Grade
        {
            Name = "Grade 1",
            SchoolId =school.Id ,
            Description = "First grade of primary school",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        },
        new Grade
        {
            Name = "Grade 2",
            SchoolId = school.Id,
             Description = "Second grade of primary school",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        },
        new Grade
        {
            Name = "Grade 3",
            SchoolId = school.Id,
             Description = "third grade of primary school",
            IsActive = true,
             CreatedAt = DateTime.UtcNow,
        },
        new Grade
        {
            Name = "Grade 4",
            SchoolId = school.Id,
             Description = "fourth grade of primary school",
            IsActive = true,
             CreatedAt = DateTime.UtcNow,
        },
        new Grade
        {
            Name = "Grade 5",
            SchoolId = school.Id,
             Description = "fifth grade of primary school",
            IsActive = true,
             CreatedAt = DateTime.UtcNow,
        },
    };

        await context.Grades.AddRangeAsync(grades);

        await context.SaveChangesAsync();
    }

    private static async Task SeedSubjectsAsync(ApplicationDbContext context)
    {
        if (await context.Subjects.AnyAsync())
            return;

        var school = await context.Schools.FirstOrDefaultAsync();

        var subjects = new List<Subject>
    {
        new Subject
        {
            Name = "Mathematics",
            SchoolId = school.Id ,
             CreatedAt = DateTime.UtcNow,
        },
        new Subject
        {
            Name = "English",
            SchoolId = school.Id ,
             CreatedAt = DateTime.UtcNow,
        },
        new Subject
        {
            Name = "Science",
            SchoolId = school.Id ,
             CreatedAt = DateTime.UtcNow,
        },
        new Subject
        {
            Name = "Arabic",
            SchoolId = school.Id ,
             CreatedAt = DateTime.UtcNow,
        }
    };

        await context.Subjects.AddRangeAsync(subjects);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[]
     {
     new ApplicationRole
        {
            Name = "OrganizationAdmin",
            Description = "Manages the organization and all schools.",
            Status = RoleStatus.Active
        },

        new ApplicationRole
        {
            Name = "SchoolAdmin",
            Description = "Manages a school, teachers and students.",
            Status = RoleStatus.Active
        },

        new ApplicationRole
        {
            Name = "Teacher",
            Description = "Manages classes",
            Status = RoleStatus.Active
        },

        new ApplicationRole
        {
            Name = "Parent",
            Description = "Can view children information",
            Status = RoleStatus.Active
        },

        new ApplicationRole
        {
            Name = "Student",
            Description = "Can view profile, subjects, attendance and results.",
            Status = RoleStatus.Active
        }
};

        foreach (var roledata in roles)
        {
            if (!await roleManager.RoleExistsAsync(roledata.Name!))
            {
                // Create Role
                var role = new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = roledata.Name,
                    Description = roledata.Description
                };

                // Add Role
                var result = await roleManager.CreateAsync(role);

                // if faild
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"{error.Code}: {error.Description}");
                    }
                }
            }
        }
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        if (await context.Permissions.AnyAsync())
            return;

        var role = await context.Roles.FirstOrDefaultAsync();

        var permissions = new List<Permission>
    {
        new Permission
        {
            Name = "School.Read",
            Description = "View schools",
             CreatedAt = DateTime.UtcNow,
        },
        new Permission
        {
            Name = "School.Create",
            Description = "Create schools",
             CreatedAt = DateTime.UtcNow,
        },
        new Permission
        {
            Name = "School.Update",
            Description = "Update schools",
             CreatedAt = DateTime.UtcNow,
        },
        new Permission
        {
            Name = "School.Delete",
            Description = "Delete schools",
             CreatedAt = DateTime.UtcNow,
        },

        new Permission
        {
            Name = "Student.Read",
            Description = "View students" ,
             CreatedAt = DateTime.UtcNow,
        },

        new Permission
        {
            Name = "Student.Create",
            Description = "Create students" ,
             CreatedAt = DateTime.UtcNow,
        }
    };

        await context.Permissions.AddRangeAsync(permissions);

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager,
                                             ApplicationDbContext context)
    {
        var existingUser = await userManager.FindByEmailAsync("admin@education.com");

        if (existingUser != null)
            return;

        var school = await context.Schools.FirstOrDefaultAsync();

        var grade = await context.Grades.FirstOrDefaultAsync();

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),

            FirstName = "System ",
            LastName = "Administrator",
            Address = "1 Admin St, Cairo, 11111, Egypt",
            DateOfBirth = new DateTime(1990, 1, 1),

            CreatedAt = DateTime.UtcNow,

            UserName = "admin",
            Email = "admin@education.com",

            SchoolId = school!.Id,
            GradeId = grade!.Id,
        };

        var result = await userManager.CreateAsync(admin, "Admin@12345");

        if (!result.Succeeded) // if not succeed
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine(
                    $"{error.Code}: {error.Description}");
            }

            return;
        }

        await userManager.AddToRoleAsync(
            admin,
            "OrganisationAdmin");
    }
}