using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IEmpleadoRepository : IRepository<Empleado>
{
    Task<Empleado?> GetByDocumentoAsync(string documento);
    Task<Empleado?> GetByEmailAsync(string email);
    Task<bool> DocumentoExistsAsync(string documento);
    Task<bool> EmailExistsAsync(string email);
    Task<int> GetTotalCountAsync();
    Task<int> GetCountByEstadoAsync(string estado);
    Task<Dictionary<string, int>> GetCountByDepartamentoAsync();
}

public interface IDepartamentoRepository : IRepository<Departamento>
{
    // Métodos específicos si los hay
}