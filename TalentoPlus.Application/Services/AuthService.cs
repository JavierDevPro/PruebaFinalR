using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services;

public class AuthService : IAuthService
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IConfiguration _configuration;
    
    public AuthService(IEmpleadoRepository empleadoRepository, IConfiguration configuration)
    {
        _empleadoRepository = empleadoRepository;
        _configuration = configuration;
    }
    
    public async Task<AuthResponseDto> LoginEmpleadoAsync(LoginEmpleadoDto loginDto)
    {
        if (string.IsNullOrEmpty(loginDto.Documento) && string.IsNullOrEmpty(loginDto.Email))
            throw new ArgumentException("Documento o Email requerido");
        
        Domain.Entities.Empleado? empleado = null;
        
        if (!string.IsNullOrEmpty(loginDto.Documento))
            empleado = await _empleadoRepository.GetByDocumentoAsync(loginDto.Documento);
        else if (!string.IsNullOrEmpty(loginDto.Email))
            empleado = await _empleadoRepository.GetByEmailAsync(loginDto.Email);
        
        if (empleado == null || empleado.Role != "Empleado")
            throw new UnauthorizedAccessException("Credenciales inválidas o no es empleado");
        
        if (string.IsNullOrEmpty(empleado.PasswordHash) || 
            !BCrypt.Net.BCrypt.Verify(loginDto.Password, empleado.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas");
        
        var token = GenerateJwtToken(empleado);
        
        var empleadoDto = new EmpleadoDto
        {
            Id = empleado.Id,
            Documento = empleado.Documento,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            Email = empleado.Email,
            DepartamentoNombre = empleado.Departamento?.Nombre ?? string.Empty
        };
        
        return new AuthResponseDto
        {
            Token = token,
            Expires = DateTime.UtcNow.AddMinutes(120),
            Empleado = empleadoDto
        };
    }
    
    public async Task<bool> LoginAdminAsync(string email, string password)
    {
        var empleado = await _empleadoRepository.GetByEmailAsync(email);
        
        if (empleado == null || empleado.Role != "Admin")
            return false;
        
        return BCrypt.Net.BCrypt.Verify(password, empleado.PasswordHash);
    }
    
    private string GenerateJwtToken(Domain.Entities.Empleado empleado)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, empleado.Documento),
            new Claim(JwtRegisteredClaimNames.Email, empleado.Email),
            new Claim("empleadoId", empleado.Id.ToString()),
            new Claim("role", empleado.Role)
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(120),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}