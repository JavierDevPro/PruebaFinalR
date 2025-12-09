using TalentoPlus.Application.DTOs;

namespace TalentoPlus.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginEmpleadoAsync(LoginEmpleadoDto loginDto);
    Task<bool> LoginAdminAsync(string email, string password);
}