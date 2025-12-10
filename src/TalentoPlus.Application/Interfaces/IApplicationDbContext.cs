using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Employee> Employees { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
