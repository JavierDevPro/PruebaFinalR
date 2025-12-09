using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmpleadoRepository : IRepository<Empleado>
{
    Task<Empleado?> GetByDocumentoAsync(string documento);
    Task<Empleado?> GetByEmailAsync(string email);
    Task<Empleado?> GetWithUsuarioAsync(int id);
    Task<bool> DocumentoExistsAsync(string documento);
    Task<bool> EmailExistsAsync(string email);
    Task<int> GetTotalCountAsync();
    Task<int> GetCountByEstadoAsync(string estado);
    Task<Dictionary<string, int>> GetCountByDepartamentoAsync();
    Task<IEnumerable<Empleado>> GetByDepartamentoIdAsync(int departamentoId);
}