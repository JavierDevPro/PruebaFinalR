namespace TalentoPlus.Application.DTOs;

public class EmpleadoDto
{
    public int Id { get; set; }
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal Salario { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string NivelEducativo { get; set; } = string.Empty;
    public string PerfilProfesional { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
}

public class CreateEmpleadoDto
{
    public string Documento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal Salario { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string Estado { get; set; } = "Activo";
    public string NivelEducativo { get; set; } = "Profesional";
    public string PerfilProfesional { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Empleado";
    public int DepartamentoId { get; set; }
}

public class UpdateEmpleadoDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal Salario { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string NivelEducativo { get; set; } = string.Empty;
    public string PerfilProfesional { get; set; } = string.Empty;
    public int DepartamentoId { get; set; }
}

public class LoginEmpleadoDto
{
    public string Documento { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public EmpleadoDto Empleado { get; set; } = null!;
}