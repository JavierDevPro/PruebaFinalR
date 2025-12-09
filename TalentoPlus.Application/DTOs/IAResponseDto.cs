namespace TalentoPlus.Application.DTOs;

public class IAResponseDto
{
    public string Pregunta { get; set; } = string.Empty;
    public string Respuesta { get; set; } = string.Empty;
    public object? Datos { get; set; }
}