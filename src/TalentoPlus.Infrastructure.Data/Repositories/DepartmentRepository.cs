using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Infrastructure.Data.Context;

namespace TalentoPlus.Infrastructure.Data.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<DepartmentEnum?> GetByNameAsync(string name)
    {
        if (Enum.TryParse<DepartmentEnum>(name, true, out var result))
        {
            return Task.FromResult<DepartmentEnum?>(result);
        }
        return Task.FromResult<DepartmentEnum?>(null);
    }

    public Task<List<DepartmentEnum>> GetAllAsync()
    {
        var values = Enum.GetValues<DepartmentEnum>().ToList();
        return Task.FromResult(values);
    }

    public Task<DepartmentEnum> AddAsync(DepartmentEnum department)
    {
        return Task.FromResult(department);
    }

    public Task UpdateAsync(DepartmentEnum department)
    {
        return Task.CompletedTask;
    }
}
