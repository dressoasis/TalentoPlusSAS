using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Infrastructure.Identity.Identity;

namespace TalentoPlus.Infrastructure.Identity.Seed;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        DbContext db)
    {
        string[] roles = { "Admin", "User" };

        // ==========================
        // CREAR ROLES
        // ===========================
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // ==========================
        // CREAR USUARIO ADMIN
        // ===========================
        var adminEmail = "admin@talentoplus.com";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "Administrador General",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123*");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // ==========================
        // CREAR EMPLOYEE DEL ADMIN
        // ===========================
        if (adminUser.EmployeeId == null)
        {
            var adminEmployee = new Employee
            {
                FirstName = "Admin",
                LastName = "Principal",
                BirthDate = DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc),
                Email = adminEmail,
                Phone = "0000000000",
                Address = "Sistema",
                Salary = 0,
                HireDate = DateTime.UtcNow,
                ProfessionalProfile = "Administrador del Sistema",

                Department = DepartmentEnum.Tecnologia,
                Position = Position.Administrador,
                EducationLevel = EducationLevelEnum.Profesional,
                Status = EmploymentStatus.Activo
            };

            // Insertar Employee
            db.Set<Employee>().Add(adminEmployee);
            await db.SaveChangesAsync();

            // Asociar AppUser → Employee
            adminUser.EmployeeId = adminEmployee.Id;
            await userManager.UpdateAsync(adminUser);
        }
    }
}
