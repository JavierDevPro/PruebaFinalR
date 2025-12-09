using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class DepartamentoRepository : IRepository<Departamento>
{
    private readonly AppDbContext _context;
    
    public DepartamentoRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Departamento>> GetAllAsync()
    {
        return await _context.Departamentos.ToListAsync();
    }
    
    public async Task<Departamento?> GetByIdAsync(int id)
    {
        return await _context.Departamentos.FindAsync(id);
    }
    
    public async Task<Departamento> CreateAsync(Departamento entity)
    {
        _context.Departamentos.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(Departamento entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Departamento entity)
    {
        _context.Departamentos.Remove(entity);
        await _context.SaveChangesAsync();
    }
}