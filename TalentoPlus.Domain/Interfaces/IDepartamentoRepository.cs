using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Domain.Interfaces;

public interface IDepartamentoRepository
{
    Task<IEnumerable<Departamento>> GetAllAsync();
    Task<Departamento?> GetByIdAsync(int id);
    Task<Departamento> CreateAsync(Departamento entity);
    Task UpdateAsync(Departamento entity);
    Task DeleteAsync(Departamento entity);
}