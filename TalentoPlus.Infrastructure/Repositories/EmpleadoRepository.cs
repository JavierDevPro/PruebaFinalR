using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly AppDbContext _context;
    
    public EmpleadoRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Empleado>> GetAllAsync()
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Usuario)
            .ToListAsync();
    }
    
    public async Task<Empleado?> GetByIdAsync(int id)
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<Empleado> CreateAsync(Empleado entity)
    {
        _context.Empleados.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(Empleado entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Empleado entity)
    {
        _context.Empleados.Remove(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Empleado?> GetByDocumentoAsync(string documento)
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Documento == documento);
    }
    
    public async Task<Empleado?> GetByEmailAsync(string email)
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Email == email);
    }
    
    public async Task<Empleado?> GetWithUsuarioAsync(int id)
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Usuario)
            .ThenInclude(u => u!.Role)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<bool> DocumentoExistsAsync(string documento)
    {
        return await _context.Empleados.AnyAsync(e => e.Documento == documento);
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Empleados.AnyAsync(e => e.Email == email);
    }
    
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Empleados.CountAsync();
    }
    
    public async Task<int> GetCountByEstadoAsync(string estado)
    {
        return await _context.Empleados.CountAsync(e => e.Estado == estado);
    }
    
    public async Task<Dictionary<string, int>> GetCountByDepartamentoAsync()
    {
        return await _context.Empleados
            .Include(e => e.Departamento)
            .GroupBy(e => e.Departamento!.Nombre)
            .Select(g => new { Departamento = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Departamento, x => x.Count);
    }
    
    public async Task<IEnumerable<Empleado>> GetByDepartamentoIdAsync(int departamentoId)
    {
        return await _context.Empleados
            .Where(e => e.DepartamentoId == departamentoId)
            .Include(e => e.Departamento)
            .ToListAsync();
    }
}