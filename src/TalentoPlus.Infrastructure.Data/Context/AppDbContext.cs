using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Infrastructure.Identity.Identity;
using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Infrastructure.Data.Context;

public class AppDbContext : IdentityDbContext<AppUser>, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ============================================================
        // ENUMS → Guardar como INT en PostgreSQL
        // ============================================================
        builder.Entity<Employee>()
            .Property(e => e.Department)
            .HasConversion<int>();

        builder.Entity<Employee>()
            .Property(e => e.Position)
            .HasConversion<int>();

        builder.Entity<Employee>()
            .Property(e => e.EducationLevel)
            .HasConversion<int>();

        builder.Entity<Employee>()
            .Property(e => e.Status)
            .HasConversion<int>();


        // ============================================================
        builder.Entity<AppUser>()
            .HasOne(u => u.Employee)
            .WithOne()
            .HasForeignKey<AppUser>(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
