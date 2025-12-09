using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Domain.Entities;

public class Departamento
{
    public int Id { get; set; }
    
    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;
    
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}