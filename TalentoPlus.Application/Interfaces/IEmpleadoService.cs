using TalentoPlus.Application.DTOs;

namespace TalentoPlus.Application.Interfaces;

public interface IEmpleadoService
{
    Task<IEnumerable<EmpleadoDto>> GetAllEmpleadosAsync();
    Task<EmpleadoDto?> GetEmpleadoByIdAsync(int id);
    Task<EmpleadoDto> CreateEmpleadoAsync(CreateEmpleadoDto dto);
    Task UpdateEmpleadoAsync(int id, UpdateEmpleadoDto dto);
    Task DeleteEmpleadoAsync(int id);
    Task<byte[]> GeneratePdfAsync(int empleadoId);
    Task ImportFromExcelAsync(Stream excelStream);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}