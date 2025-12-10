using TalentoPlus.Application.DTOs;

namespace TalentoPlus.Application.Interfaces;

public interface IPdfService
{
    byte[] GenerateHojaDeVida(EmpleadoDto empleado);
}