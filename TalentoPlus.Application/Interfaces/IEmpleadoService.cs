using TalentoPlus.Application.DTOs;

namespace TalentoPlus.Application.Interfaces;

public interface IEmpleadoService
{
    Task<IEnumerable<EmpleadoDto>> GetAllAsync();
    Task<EmpleadoDto?> GetByIdAsync(int id);
    Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto);
    Task UpdateAsync(int id, UpdateEmpleadoDto dto);
    Task DeleteAsync(int id);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}