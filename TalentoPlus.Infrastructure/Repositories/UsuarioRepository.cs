using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Data;

namespace TalentoPlus.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    
    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Role)
            .Include(u => u.Empleado)
            .ToListAsync();
    }
    
    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Role)
            .Include(u => u.Empleado)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<Usuario> CreateAsync(Usuario entity)
    {
        _context.Usuarios.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(Usuario entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Usuario entity)
    {
        _context.Usuarios.Remove(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include(u => u.Role)
            .Include(u => u.Empleado)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }
    
    public async Task<Usuario?> GetWithEmpleadoAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.Role)
            .Include(u => u.Empleado)
            .ThenInclude(e => e!.Departamento)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}