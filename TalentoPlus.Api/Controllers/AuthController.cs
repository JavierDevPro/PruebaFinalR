using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterEmpleadoDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterEmpleadoAsync(registerDto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenDto refreshDto)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshDto);
            return Ok(result);
        }
        catch (SecurityTokenException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] LoginDto logoutDto)
    {
        try
        {
            var result = await _authService.LogoutAsync(logoutDto.Email);
            if (!result)
                return BadRequest("No se pudo cerrar sesión");
                
            return Ok(new { message = "Sesión cerrada correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}