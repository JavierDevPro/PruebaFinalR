using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IDepartamentoRepository _departamentoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    
    public EmpleadoService(
        IEmpleadoRepository empleadoRepository,
        IDepartamentoRepository departamentoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _empleadoRepository = empleadoRepository;
        _departamentoRepository = departamentoRepository;
        _usuarioRepository = usuarioRepository;
    }
    
    public async Task<IEnumerable<EmpleadoDto>> GetAllAsync()
    {
        var empleados = await _empleadoRepository.GetAllAsync();
        return empleados.Select(MapToDto);
    }
    

    public async Task<EmpleadoDto?> GetByIdAsync(int id)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(id);
        return empleado != null ? MapToDto(empleado) : null;
    }
    
    public async Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto)
    {
        // Validar
        if (await _empleadoRepository.DocumentoExistsAsync(dto.Documento))
            throw new ArgumentException($"Documento {dto.Documento} ya existe");
            
        if (await _empleadoRepository.EmailExistsAsync(dto.Email))
            throw new ArgumentException($"Email {dto.Email} ya existe");
        
        // Verificar departamento existe
        var departamento = await _departamentoRepository.GetByIdAsync(dto.DepartamentoId);
        if (departamento == null)
            throw new ArgumentException($"Departamento con ID {dto.DepartamentoId} no existe");
        
        var empleado = new Empleado
        {
            Documento = dto.Documento,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            FechaNacimiento = dto.FechaNacimiento,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Cargo = dto.Cargo,
            Salario = dto.Salario,
            FechaIngreso = dto.FechaIngreso,
            Estado = dto.Estado,
            NivelEducativo = dto.NivelEducativo,
            PerfilProfesional = dto.PerfilProfesional,
            DepartamentoId = dto.DepartamentoId,
            UsuarioId = null
        };
        
        var created = await _empleadoRepository.CreateAsync(empleado);
        return MapToDto(created);
    }
    
    public async Task UpdateAsync(int id, UpdateEmpleadoDto dto)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(id);
        if (empleado == null)
            throw new ArgumentException($"Empleado con ID {id} no encontrado");
        
        // Validar departamento si cambió
        if (empleado.DepartamentoId != dto.DepartamentoId)
        {
            var departamento = await _departamentoRepository.GetByIdAsync(dto.DepartamentoId);
            if (departamento == null)
                throw new ArgumentException($"Departamento con ID {dto.DepartamentoId} no existe");
        }
        
        empleado.Nombres = dto.Nombres;
        empleado.Apellidos = dto.Apellidos;
        empleado.Direccion = dto.Direccion;
        empleado.Telefono = dto.Telefono;
        empleado.Cargo = dto.Cargo;
        empleado.Salario = dto.Salario;
        empleado.Estado = dto.Estado;
        empleado.NivelEducativo = dto.NivelEducativo;
        empleado.PerfilProfesional = dto.PerfilProfesional;
        empleado.DepartamentoId = dto.DepartamentoId;
        
        await _empleadoRepository.UpdateAsync(empleado);
    }
    
    public async Task DeleteAsync(int id)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(id);
        if (empleado == null)
            throw new ArgumentException($"Empleado con ID {id} no encontrado");
        
        // Verificar si tiene usuario asociado
        if (empleado.UsuarioId.HasValue)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(empleado.UsuarioId.Value);
            if (usuario != null)
            {
                // Eliminar usuario primero
                await _usuarioRepository.DeleteAsync(usuario);
            }
        }
        
        await _empleadoRepository.DeleteAsync(empleado);
    }
    
    public async Task<byte[]> GeneratePdfAsync(int empleadoId)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(empleadoId);
        if (empleado == null)
            throw new ArgumentException($"Empleado con ID {empleadoId} no encontrado");
        
        // PDF simple temporal
        var pdfContent = $"HOJA DE VIDA\n" +
                        $"============\n" +
                        $"Documento: {empleado.Documento}\n" +
                        $"Nombre: {empleado.Nombres} {empleado.Apellidos}\n" +
                        $"Email: {empleado.Email}\n" +
                        $"Cargo: {empleado.Cargo}\n" +
                        $"Departamento: {empleado.Departamento?.Nombre}\n" +
                        $"Salario: ${empleado.Salario:N0}\n" +
                        $"Estado: {empleado.Estado}\n" +
                        $"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}";
        
        return System.Text.Encoding.UTF8.GetBytes(pdfContent);
    }
    
    public async Task ImportFromExcelAsync(Stream excelStream)
    {
        // Implementación básica - solo para demo
        throw new NotImplementedException("Importación de Excel pendiente");
    }
    
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalEmpleados = await _empleadoRepository.GetTotalCountAsync(),
            EmpleadosVacaciones = await _empleadoRepository.GetCountByEstadoAsync("Vacaciones"),
            EmpleadosActivos = await _empleadoRepository.GetCountByEstadoAsync("Activo"),
            EmpleadosPorDepartamento = await _empleadoRepository.GetCountByDepartamentoAsync()
        };
    }
    
    private EmpleadoDto MapToDto(Empleado empleado)
    {
        return new EmpleadoDto
        {
            Id = empleado.Id,
            Documento = empleado.Documento,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            FechaNacimiento = empleado.FechaNacimiento,
            Direccion = empleado.Direccion,
            Telefono = empleado.Telefono,
            Email = empleado.Email,
            Cargo = empleado.Cargo,
            Salario = empleado.Salario,
            FechaIngreso = empleado.FechaIngreso,
            Estado = empleado.Estado,
            NivelEducativo = empleado.NivelEducativo,
            PerfilProfesional = empleado.PerfilProfesional,
            DepartamentoId = empleado.DepartamentoId,
            DepartamentoNombre = empleado.Departamento?.Nombre ?? "No asignado",
            TieneUsuario = empleado.UsuarioId.HasValue
        };
    }
}