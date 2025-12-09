using TalentoPlus.Application.DTOs;

namespace TalentoPlus.Application.Interfaces;

public interface IAuthService
{
    // Métodos CORRECTOS (igual que en el servicio)
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterEmpleadoAsync(RegisterEmpleadoDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshDto);
    Task<bool> LogoutAsync(string email);
    
    // OPCIONAL: Si necesitas login admin separado
    // Task<bool> LoginAdminAsync(string email, string password);
}