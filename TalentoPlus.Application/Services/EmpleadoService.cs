using AutoMapper;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Domain.Entities;


namespace TalentoPlus.Application.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _repository;
    private readonly IMapper _mapper;
    
    public EmpleadoService(IEmpleadoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<EmpleadoDto>> GetAllAsync()
    {
        var empleados = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<EmpleadoDto>>(empleados);
    }
    
    public async Task<EmpleadoDto?> GetByIdAsync(int id)
    {
        var empleado = await _repository.GetByIdAsync(id);
        return _mapper.Map<EmpleadoDto?>(empleado);
    }
    
    public async Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto)
    {
        if (await _repository.DocumentoExistsAsync(dto.Documento))
            throw new Exception($"Documento {dto.Documento} ya existe");
            
        if (await _repository.EmailExistsAsync(dto.Email))
            throw new Exception($"Email {dto.Email} ya existe");
            
        var empleado = _mapper.Map<Empleado>(dto);
        
        // Hash password
        if (!string.IsNullOrEmpty(dto.Password))
        {
            empleado.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }
        
        var created = await _repository.CreateAsync(empleado);
        return _mapper.Map<EmpleadoDto>(created);
    }
    
    public async Task UpdateAsync(int id, UpdateEmpleadoDto dto)
    {
        var empleado = await _repository.GetByIdAsync(id);
        if (empleado == null)
            throw new Exception($"Empleado con ID {id} no encontrado");
            
        _mapper.Map(dto, empleado);
        await _repository.UpdateAsync(empleado);
    }
    
    public async Task DeleteAsync(int id)
    {
        var empleado = await _repository.GetByIdAsync(id);
        if (empleado == null)
            throw new Exception($"Empleado con ID {id} no encontrado");
            
        await _repository.DeleteAsync(empleado);
    }
    
    // Métodos para dashboard (necesitan repositorio extendido)
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        // Necesitamos extender el repositorio para estas consultas
        // Por ahora retornamos vacío
        return new DashboardStatsDto();
    }
}