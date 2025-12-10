using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Data.Context;

namespace TalentoPlus.Infrastructure.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    // ============================================
    // GET ALL
    // ============================================
    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .ToListAsync();
    }

    // ============================================
    // GET BY ID
    // ============================================
    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // ============================================
    // INSERT
    // ============================================
    public async Task AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    // ============================================
    // UPDATE
    // ============================================
    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    // ============================================
    // DELETE
    // ============================================
    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
