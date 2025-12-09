using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpire { get; set; }
    
    // Relaciones
    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
    
    // Relación 1:1 con Empleado
    public int? EmpleadoId { get; set; }
    public virtual Empleado? Empleado { get; set; }
}