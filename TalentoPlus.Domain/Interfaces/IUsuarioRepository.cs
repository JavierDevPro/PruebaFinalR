using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<Usuario?> GetWithEmpleadoAsync(int id);
}