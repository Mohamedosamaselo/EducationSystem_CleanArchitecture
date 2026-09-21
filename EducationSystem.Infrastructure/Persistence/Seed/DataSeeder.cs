using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        await SeedRolesAsync(roleManager);

        await SeedUsersAsync(userManager, roleManager);

        await SeedUserRoleAsync(userManager, roleManager);

        await SeedRoleClaimsAsync(roleManager);
    }

    private static async Task SeedOrganizationAsync(ApplicationDbContext context)
    {
        if (await context.Organisations.AnyAsync())
            return;

        var organizations = new List<Organisation>
        {
            new Organisation
            {
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

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager,
                                            RoleManager<ApplicationRole> roleManager)
    {
        if (await userManager.Users.AnyAsync())
            return;

        var users = new List<(string UserName, string Email, string Password, string Role, string FirstName, string LastName, string Address, Guid? SchoolId, Guid? GradeId)>
    {
        ("orgadmin", "orgadmin@example.com", "OrgAdmin@123", "OrganisationAdmin", "Ali", "Hassan","Cairo" ,null, null),
        ("schooladmin", "schooladmin@example.com", "SchoolAdmin@123", "SchoolAdmin", "Sara", "Mahmoud","ElRehab", Guid.Parse("31464c0b-4b25-4d99-a80d-23add5f25565"), null),
        ("teacher", "teacher@example.com", "Teacher@123", "Teacher", "Omar", "Khaled","Nasr city ", Guid.Parse("005ac0e9-5468-4a77-aecc-741c85a4e7b9"), Guid.Parse("65f3d6d6-754a-4e03-b6c2-700782aa8050")),
        ("student", "student@example.com", "Student@123", "Student", "Mona", "Ibrahim", "Helioplis",Guid.Parse("e41b0fdf-11ea-4453-9358-b66c3f452844"), Guid.Parse("844d8e13-75f8-45da-b3bc-a3203f8d2cd1")),
        ("parent", "parent@example.com", "Parent@123", "Parent", "Hussein", "Ali","Alx" ,null, null)
    };

        foreach (var (userName, email, password, roleName, firstName, lastName, address, schoolId, gradeId) in users)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
                continue;

            var newUser = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                DateOfBirth = DateTime.UtcNow.AddYears(-20), // dummy DOB
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                SchoolId = schoolId,
                GradeId = gradeId
            };

            var result = await userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"{error.Code}: {error.Description}");
                }
                continue;
            }

            if (await roleManager.RoleExistsAsync(roleName))
            {
                await userManager.AddToRoleAsync(newUser, roleName);
            }
        }
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        if (await roleManager.Roles.AnyAsync())
            return;

        var roles = new List<ApplicationRole>
    {
        new ApplicationRole
        {
            Name = "OrganisationAdmin",
            IsDefault = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        },

        new ApplicationRole
        {
            Name = "SchoolAdmin",
            IsDefault = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        },

        new ApplicationRole
        {
            Name = "Teacher",
            IsDefault = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        },

        new ApplicationRole
        {
            Name = "Student",
            IsDefault = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        },

         new ApplicationRole
        {
            Name = "Parent",
            IsDefault = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        }
    };

        foreach (var role in roles)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
                continue;

            var existingRole = await roleManager.FindByNameAsync(role.Name);

            if (existingRole != null)
                continue;

            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(
                        $"{error.Code}: {error.Description}");
                }
            }
        }
    }

    private static async Task SeedUserRoleAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        if (await userManager.Users.AnyAsync())
            return;

        if (!await roleManager.Roles.AnyAsync())
            return;

        var userRoles = new List<(string Email, string Role)>
        {
                ("orgadmin@example.com", "OrganisationAdmin"),
                 ("schooladmin@example.com", "SchoolAdmin"),
                 ("teacher@example.com", "Teacher"),
                 ("student@example.com", "Student"),
                 ("parent@example.com", "Parent")
        };

        foreach (var (email, roleName) in userRoles)
        {
            // find user by email and check on user
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                continue;

            var role = await roleManager.RoleExistsAsync(roleName);
            if (!role) // if role not found
                continue;

            // check if user is already in role or not
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                var result = await userManager.AddToRoleAsync(user, roleName);
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

    private static async Task SeedRoleClaimsAsync(
        RoleManager<ApplicationRole> roleManager)
    {
        var organisationAdmin =
            await roleManager.FindByNameAsync("OrganisationAdmin");

        if (organisationAdmin is not null)
        {
            await AddPermissionsToRoleAsync(
                roleManager,
                organisationAdmin,
                new[]
                {
                Permissions.GetSchools,
                Permissions.AddSchools,
                Permissions.UpdateSchools,
                Permissions.DeleteSchools,

                Permissions.GetGrades,
                Permissions.AddGrades,
                Permissions.UpdateGrades,
                Permissions.DeleteGrades,

                Permissions.GetSubjects,
                Permissions.AddSubjects,
                Permissions.UpdateSubjects,
                Permissions.DeleteSubjects
                });
        }

        var schoolAdmin =
            await roleManager.FindByNameAsync("SchoolAdmin");

        if (schoolAdmin is not null)
        {
            await AddPermissionsToRoleAsync(
                roleManager,
                schoolAdmin,
                new[]
                {
                Permissions.GetSchools,
                Permissions.UpdateSchools,

                Permissions.GetGrades,
                Permissions.AddGrades,
                Permissions.UpdateGrades,
                Permissions.DeleteGrades,

                Permissions.GetSubjects,
                Permissions.AddSubjects,
                Permissions.UpdateSubjects,
                Permissions.DeleteSubjects
                });
        }

        var teacher =
            await roleManager.FindByNameAsync("Teacher");

        if (teacher is not null)
        {
            await AddPermissionsToRoleAsync(
                roleManager,
                teacher,
                new[]
                {
                Permissions.GetSchools,
                Permissions.GetGrades,
                Permissions.GetSubjects
                });
        }

        var student =
            await roleManager.FindByNameAsync("Student");

        if (student is not null)
        {
            await AddPermissionsToRoleAsync(
                roleManager,
                student,
                new[]
                {
                Permissions.GetSchools,
                Permissions.GetGrades,
                Permissions.GetSubjects
                });
        }
    }

    // helper Method
    private static async Task AddPermissionsToRoleAsync(
        RoleManager<ApplicationRole> roleManager,
        ApplicationRole role,
        IEnumerable<string> permissions)
    {
        var existingClaims = await roleManager.GetClaimsAsync(role);

        foreach (var permission in permissions)
        {
            var alreadyExists = existingClaims.Any(c =>
                c.Type == "permission" &&
                c.Value == permission);

            if (alreadyExists)
                continue;

            var result = await roleManager.AddClaimAsync(
                role,
                new Claim("permission", permission));

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(
                        $"{error.Code}: {error.Description}");
                }
            }
        }
    }
}