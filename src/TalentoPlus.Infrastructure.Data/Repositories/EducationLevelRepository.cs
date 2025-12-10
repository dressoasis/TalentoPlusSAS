using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Infrastructure.Data.Context;

namespace TalentoPlus.Infrastructure.Data.Repositories;

public class EducationLevelRepository : IEducationLevelRepository
{
    private readonly AppDbContext _context;

    public EducationLevelRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<EducationLevelEnum?> GetByNameAsync(string name)
    {
        if (Enum.TryParse<EducationLevelEnum>(name, true, out var result))
        {
            return Task.FromResult<EducationLevelEnum?>(result);
        }
        return Task.FromResult<EducationLevelEnum?>(null);
    }

    public Task<List<EducationLevelEnum>> GetAllAsync()
    {
        var values = Enum.GetValues<EducationLevelEnum>().ToList();
        return Task.FromResult(values);
    }

    public Task<EducationLevelEnum> AddAsync(EducationLevelEnum educationLevel)
    {
        // Enums are static, we cannot add to them at runtime.
        // We just return the value to simulate success.
        return Task.FromResult(educationLevel);
    }
}
