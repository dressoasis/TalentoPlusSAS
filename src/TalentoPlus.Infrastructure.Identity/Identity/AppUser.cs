using Microsoft.AspNetCore.Identity;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Identity.Identity;

public class AppUser : IdentityUser
{
    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }  // Computed or legacy field
}
