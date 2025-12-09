using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class RoleRepository : IRepository<Role>
{
    private readonly AppDbContext _context;
    
    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }
    
    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }
    
    public async Task<Role> CreateAsync(Role entity)
    {
        _context.Roles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(Role entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Role entity)
    {
        _context.Roles.Remove(entity);
        await _context.SaveChangesAsync();
    }
}