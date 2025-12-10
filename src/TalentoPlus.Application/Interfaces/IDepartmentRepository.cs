using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.Interfaces;

public interface IDepartmentRepository
{
    Task<DepartmentEnum?> GetByNameAsync(string name);
    Task<List<DepartmentEnum>> GetAllAsync();
    Task<DepartmentEnum> AddAsync(DepartmentEnum department);
    Task UpdateAsync(DepartmentEnum department);
}
