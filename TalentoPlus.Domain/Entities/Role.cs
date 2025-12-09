using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    
    [Required, MaxLength(50)]
    public string Nombre { get; set; } = string.Empty; // "Admin", "Empleado"
    
    [MaxLength(200)]
    public string Descripcion { get; set; } = string.Empty;
    
    // Navigation
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}