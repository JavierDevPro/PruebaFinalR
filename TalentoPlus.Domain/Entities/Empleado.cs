using System;
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
    
    // Enums como strings para DB
    public string Estado { get; set; } = "Activo"; // "Activo", "Vacaciones", "Inactivo"
    public string NivelEducativo { get; set; } = "Profesional"; // "Bachiller", "Tecnologo", etc.
    
    public string PerfilProfesional { get; set; } = string.Empty;
    
    // AUTENTICACIÓN
    public string? PasswordHash { get; set; } // Hashed password para empleados
    public string Role { get; set; } = "Empleado"; // "Admin" o "Empleado"
    
    // RELACIONES
    public int DepartamentoId { get; set; }
    public virtual Departamento? Departamento { get; set; }
}