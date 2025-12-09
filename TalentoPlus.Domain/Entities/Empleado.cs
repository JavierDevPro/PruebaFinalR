using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Domain.Entities;

public class Empleado
{
    public int Id { get; set; }
    
    [Required, MaxLength(20)]
    public string Documento { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;
    
    public DateTime FechaNacimiento { get; set; }
    
    [MaxLength(200)]
    public string Direccion { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;
    
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Cargo { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue)]
    public decimal Salario { get; set; }
    
    public DateTime FechaIngreso { get; set; }
    
    public string Estado { get; set; } = "Activo"; // "Activo", "Vacaciones", "Inactivo"
    
    public string NivelEducativo { get; set; } = "Profesional"; // "Bachiller", "Tecnologo", "Profesional", "Especializacion", "Maestria", "Doctorado"
    
    public string PerfilProfesional { get; set; } = string.Empty;
    
    // Relaciones
    public int DepartamentoId { get; set; }
    public virtual Departamento? Departamento { get; set; }
    
    // Relación con Usuario (opcional)
    public int? UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; }
}