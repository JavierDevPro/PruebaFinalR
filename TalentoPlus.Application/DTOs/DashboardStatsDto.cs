namespace TalentoPlus.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalEmpleados { get; set; }
    public int EmpleadosVacaciones { get; set; }
    public int EmpleadosActivos { get; set; }
    public Dictionary<string, int> EmpleadosPorDepartamento { get; set; } = new();
}