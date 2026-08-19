using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace EducationSystem.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(RoleManager<Role> roleManager)
    {
        var roles = new[]
        {
             new
    {
        Name = "User",
        Description = "Default user role."
    },
            new
            {
                Name = "Admin",
                Description = "Has full access to the system."
            } ,
            new
            {
                Name = "Teacher",
                Description = "Can manage students, grades, and subjects."
            },
            new
            {
                Name = "Student",
                Description = "Can view their grades, subjects, and school information."
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
}