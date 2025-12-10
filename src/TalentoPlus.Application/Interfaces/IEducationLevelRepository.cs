using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.Interfaces;

public interface IEducationLevelRepository
{
    Task<EducationLevelEnum?> GetByNameAsync(string name);
    Task<List<EducationLevelEnum>> GetAllAsync();
    Task<EducationLevelEnum> AddAsync(EducationLevelEnum educationLevel);
}