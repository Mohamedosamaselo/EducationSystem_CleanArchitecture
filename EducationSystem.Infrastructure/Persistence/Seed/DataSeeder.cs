using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EducationSystem.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<Role> roleManager)
    {
        await SeedOrganizationAsync(context);
        await SeedSchoolAsync(context);

        await SeedGradeAsync(context);

        await SeedSubjectsAsync(context);

        await SeedRolesAsync(roleManager);

        await SeedPermissionsAsync(context);

        await SeedUsersAsync(userManager, context);
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
                CreatedAt = DateTime.UtcNow,
            }
        };

        await context.AddRangeAsync(organizations);

        await context.SaveChangesAsync();
    }

    private static async Task SeedSchoolAsync(ApplicationDbContext context)
    {
        if (await context.Schools.AnyAsync())
            return;

        var organization = await context.Organisations.FirstOrDefaultAsync();

        var schools = new List<School>
    {
        new School
        {
            Name = "Cairo International School",
            Address = "Cairo",
            OrganisationId = organization.Id ,
             CreatedAt = DateTime.UtcNow,
        },
        new School
        {
            Name = "Giza Modern School",
            Address = "Giza",
            OrganisationId = organization.Id ,
             CreatedAt = DateTime.UtcNow,
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

        var grades = new List<Grade>
       {
        new Grade
        {
            Name = "Grade 1",
            SchoolId =school.Id ,
             CreatedAt = DateTime.UtcNow,
        },
        new Grade
        {
            Name = "Grade 2",
            SchoolId = school.Id,
             CreatedAt = DateTime.UtcNow,
        },
        new Grade
        {
            Name = "Grade 3",
            SchoolId = school.Id,
             CreatedAt = DateTime.UtcNow,
        }
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

    private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        var roles = new Role[]
        {
             new Role
             {
                 Name = "User",
                 Description = "Default user role." ,
                 CreatedAt =  DateTime.UtcNow,
             },
            new Role
             {
                 Name = "Admin",
                 Description = "Has full access to the system."  ,
                  CreatedAt = DateTime.UtcNow,
             } ,
             new Role
            {
                Name = "Teacher",
                Description = "Can manage students, grades, and subjects.",
                 CreatedAt = DateTime.UtcNow,
            },
            new Role
            {
                Name = "Student",
                Description = "Can view their grades, subjects, and school information." ,
                  CreatedAt = DateTime.UtcNow,
            }
        };

        foreach (var roledata in roles)
        {
            if (!await roleManager.RoleExistsAsync(roledata.Name!))
            {
                // Create Role
                var role = new Role
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
            RoleId = role!.Id ,
             CreatedAt = DateTime.UtcNow,
        },
        new Permission
        {
            Name = "School.Create",
            Description = "Create schools",
            RoleId = role.Id ,
             CreatedAt = DateTime.UtcNow,
        },
        new Permission
        {
            Name = "School.Update",
            Description = "Update schools",
            RoleId = role.Id,
             CreatedAt = DateTime.UtcNow,
        },
        new Permission
        {
            Name = "School.Delete",
            Description = "Delete schools",
            RoleId = role.Id ,
             CreatedAt = DateTime.UtcNow,
        },

        new Permission
        {
            Name = "Student.Read",
            Description = "View students" ,
             RoleId = role.Id ,
             CreatedAt = DateTime.UtcNow,
        },

        new Permission
        {
            Name = "Student.Create",
            Description = "Create students" ,
             RoleId = role.Id ,
             CreatedAt = DateTime.UtcNow,
        }
    };

        await context.Permissions.AddRangeAsync(permissions);

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager,
                                             ApplicationDbContext context)
    {
        var existingUser = await userManager
            .FindByEmailAsync("admin@education.com");

        if (existingUser != null)
            return;

        var school = await context.Schools.FirstOrDefaultAsync();

        var grade = await context.Grades.FirstOrDefaultAsync();

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),

            Name = "System Administrator",
            Address = "Cairo",
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
            "Admin");
    }
}