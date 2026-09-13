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

        await SeedSubjectsAsync(context);

        // await SeedRolesAsync(roleManager);
        // await SeedPermissionsAsync(context);
        // await SeedUsersAsync(userManager, context);
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
    Address = "123 Nile St, Cairo, 11511, Egypt",
    Email = "info@cairointernational.edu.eg",
    PhoneNumber = "+20 2 2735 1122",
    Status = SchoolStatus.Active,
    LogoUrl = "https://example.com/logos/cairo_international.png",
    OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
},
new School
{
    Name = "Alexandria Modern School",
    Address = "45 Corniche Road, Alexandria, 21500, Egypt",
    Email = "contact@alexmodern.edu.eg",
    PhoneNumber = "+20 3 4871 5566",
    Status = SchoolStatus.Active,
    LogoUrl = "https://example.com/logos/alexandria_modern.png",
    OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
},
new School
{
    Name = "Giza Excellence Academy",
    Address = "78 Pyramids Road, Giza, 12511, Egypt",
    Email = "admin@gizaexcellence.edu.eg",
    PhoneNumber = "+20 2 3377 8899",
    Status = SchoolStatus.Active,
    LogoUrl = "https://example.com/logos/giza_excellence.png",
    OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
},
new School
{
    Name = "Mansoura National School",
    Address = "32 El Gomhoria St, Mansoura, 35511, Egypt",
    Email = "info@mansouranational.edu.eg",
    PhoneNumber = "+20 50 223 4477",
    Status = SchoolStatus.Inactive,
    LogoUrl = "https://example.com/logos/mansoura_national.png",
    OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
},
new School
{
    Name = "Aswan Heritage School",
    Address = "12 Corniche El Nil, Aswan, 81511, Egypt",
    Email = "contact@aswanheritage.edu.eg",
    PhoneNumber = "+20 97 231 6688",
    Status = SchoolStatus.Active,
    LogoUrl = "https://example.com/logos/aswan_heritage.png",
    OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
},
new School
{
    Name = "Luxor Advanced Academy",
    Address = "9 Karnak St, Luxor, 85951, Egypt",
    Email = "info@luxoradvanced.edu.eg",
    PhoneNumber = "+20 95 237 9911",
    Status = SchoolStatus.Active,
    LogoUrl = "https://example.com/logos/luxor_advanced.png",
    OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
},
new School
{
    Name = "Ismailia Future School",
    Address = "56 Suez Canal St, Ismailia, 41511, Egypt",
    Email = "admin@ismailiafuture.edu.eg",
    PhoneNumber = "+20 64 391 2244",
    Status = SchoolStatus.Closed,
    LogoUrl = "https://example.com/logos/ismailia_future.png",
    OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
},
new School
{
    Name = "Tanta Modern Education School",
    Address = "21 El Bahr St, Tanta, 31511, Egypt",
    Email = "contact@tantamodern.edu.eg",
    PhoneNumber = "+20 40 331 5577",
    Status = SchoolStatus.Active,
    LogoUrl = "https://example.com/logos/tanta_modern.png",
      OrganisationId = organisation.Id,
    CreatedAt = DateTime.UtcNow
    } };

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
        if (school == null)
            throw new InvalidOperationException
                ("School not found. Please seed the school first.");

        var subjects = new List<Subject>
    {
          new Subject
    {
        Name = "Mathematics",
        SchoolId = school.Id,
        Description = "Mathematics subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "English",
        SchoolId = school.Id,
        Description = "English language and literature subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "Arabic",
        SchoolId = school.Id,
        Description = "Arabic language and literature subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "Science",
        SchoolId = school.Id,
        Description = "General science subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "Physics",
        SchoolId = school.Id,
        Description = "Physics and physical science subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "Chemistry",
        SchoolId = school.Id,
        Description = "Chemistry and laboratory science subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "Biology",
        SchoolId = school.Id,
        Description = "Biology and life science subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "History",
        SchoolId = school.Id,
        Description = "History and historical studies subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "Geography",
        SchoolId = school.Id,
        Description = "Geography and earth studies subject",
        CreatedAt = DateTime.UtcNow,
    },

    new Subject
    {
        Name = "Computer Science",
        SchoolId = school.Id,
        Description = "Computer science and programming subject",
        CreatedAt = DateTime.UtcNow,
    }
    };

        await context.Subjects.AddRangeAsync(subjects);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[]
     {  new ApplicationRole
            {
                Name = "OrganizationAdmin",
                Description = "Can manage the organization and all schools.",
                Status = RoleStatus.Active,
                 CreatedAt = DateTime.UtcNow,
            },
            new ApplicationRole
            {
                Name = "SchoolAdmin",
                Description = "Can manage a school, teachers and students.",
                Status = RoleStatus.Active,
                 CreatedAt = DateTime.UtcNow,
            },
            new ApplicationRole
            {
                Name = "Teacher",
                Description = "Can manage students, grades, and subjects.",
                Status = RoleStatus.Active,
                 CreatedAt = DateTime.UtcNow,
            },
            new ApplicationRole
            {
                Name = "Student",
                Description = "Can view their grades, subjects, and school information." ,
                Status = RoleStatus.Active,
                  CreatedAt = DateTime.UtcNow,
            },
            new ApplicationRole
            {
                Name = "Parent",
                Description = "Can view their children's grades, subjects, and school information.",
                Status = RoleStatus.Active,
                 CreatedAt = DateTime.UtcNow,
            }
};

        foreach (var roledata in roles)
        {
            // IF ROLE NOT EXIST CREATE IT
            if (!await roleManager.RoleExistsAsync(roledata.Name))
            {
                // Create Role
                var role = new ApplicationRole
                {
                    Name = roledata.Name,
                    Status = RoleStatus.Active,
                    Description = roledata.Description,
                    CreatedAt = roledata.CreatedAt
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