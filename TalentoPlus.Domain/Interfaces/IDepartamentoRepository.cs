using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IDepartamentoRepository : IRepository<Departamento> 
{
    // Ya no necesitas repetir los métodos básicos
    // Solo métodos específicos si los necesitas
    Task<Departamento?> GetByNombreAsync(string nombre);
}