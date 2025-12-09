using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;  // ← Asegúrate este using
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class DepartamentoRepository : IDepartamentoRepository
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
    
    public async Task<Departamento?> GetByNombreAsync(string nombre)
    {
        return await _context.Departamentos
            .FirstOrDefaultAsync(d => d.Nombre == nombre);
    }
}