using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IConfiguration _configuration;
    
    public AuthService(
        IUsuarioRepository usuarioRepository,
        IEmpleadoRepository empleadoRepository,
        IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _empleadoRepository = empleadoRepository;
        _configuration = configuration;
    }
    
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // 1. Buscar usuario por email
        var usuario = await _usuarioRepository.GetByEmailAsync(loginDto.Email);
        if (usuario == null)
            throw new UnauthorizedAccessException("Credenciales inválidas");
        
        // 2. Verificar password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas");
        
        // 3. Generar JWT token
        var token = GenerateJwtToken(usuario);
        
        // 4. Generar refresh token
        var refreshToken = GenerateRefreshToken();
        
        // 5. Guardar refresh token en usuario
        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpire = DateTime.UtcNow.AddDays(7);
        await _usuarioRepository.UpdateAsync(usuario);
        
        // 6. Obtener empleado si existe
        EmpleadoDto? empleadoDto = null;
        if (usuario.EmpleadoId.HasValue)
        {
            var empleado = await _empleadoRepository.GetWithUsuarioAsync(usuario.EmpleadoId.Value);
            if (empleado != null)
            {
                empleadoDto = MapToEmpleadoDto(empleado);
            }
        }
        
        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            TokenExpires = DateTime.UtcNow.AddMinutes(120),
            RefreshTokenExpires = DateTime.UtcNow.AddDays(7),
            Empleado = empleadoDto
        };
    }
    
    public async Task<AuthResponseDto> RegisterEmpleadoAsync(RegisterEmpleadoDto registerDto)
    {
        // 1. Verificar que el empleado existe en base de datos
        var empleado = await _empleadoRepository.GetByDocumentoAsync(registerDto.Documento);
        if (empleado == null)
            throw new ArgumentException($"No existe empleado con documento {registerDto.Documento}");
        
        // 2. Verificar que el email coincide con el empleado
        if (empleado.Email.ToLower() != registerDto.Email.ToLower())
            throw new ArgumentException($"El email no coincide con el registrado para este empleado");
        
        // 3. Verificar que el empleado no tenga usuario ya
        if (empleado.UsuarioId.HasValue)
            throw new ArgumentException($"Este empleado ya tiene una cuenta registrada");
        
        // 4. Verificar que el email no esté registrado
        if (await _usuarioRepository.EmailExistsAsync(registerDto.Email))
            throw new ArgumentException($"El email {registerDto.Email} ya está registrado");
        
        // 5. Crear usuario con role "Empleado" (ID 2)
        var usuario = new Usuario
        {
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            RoleId = 2, // Role "Empleado"
            EmpleadoId = empleado.Id,
            RefreshToken = null,
            RefreshTokenExpire = null
        };
        
        await _usuarioRepository.CreateAsync(usuario);
        
        // 6. Actualizar empleado con referencia al usuario
        empleado.UsuarioId = usuario.Id;
        await _empleadoRepository.UpdateAsync(empleado);
        
        // 7. Generar tokens
        var token = GenerateJwtToken(usuario);
        var refreshToken = GenerateRefreshToken();
        
        // 8. Actualizar usuario con refresh token
        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpire = DateTime.UtcNow.AddDays(7);
        await _usuarioRepository.UpdateAsync(usuario);
        
        // 9. Mapear a DTO
        var empleadoDto = MapToEmpleadoDto(empleado);
        
        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            TokenExpires = DateTime.UtcNow.AddMinutes(120),
            RefreshTokenExpires = DateTime.UtcNow.AddDays(7),
            Empleado = empleadoDto
        };
    }
    
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshDto)
    {
        // 1. Validar token expirado
        var principal = GetPrincipalFromExpiredToken(refreshDto.Token);
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(email))
            throw new SecurityTokenException("Token inválido");
        
        // 2. Buscar usuario
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario == null || usuario.RefreshToken != refreshDto.RefreshToken || 
            usuario.RefreshTokenExpire <= DateTime.UtcNow)
            throw new SecurityTokenException("Refresh token inválido o expirado");
        
        // 3. Generar nuevos tokens
        var newToken = GenerateJwtToken(usuario);
        var newRefreshToken = GenerateRefreshToken();
        
        // 4. Actualizar usuario
        usuario.RefreshToken = newRefreshToken;
        usuario.RefreshTokenExpire = DateTime.UtcNow.AddDays(7);
        await _usuarioRepository.UpdateAsync(usuario);
        
        // 5. Obtener empleado si existe
        EmpleadoDto? empleadoDto = null;
        if (usuario.EmpleadoId.HasValue)
        {
            var empleado = await _empleadoRepository.GetWithUsuarioAsync(usuario.EmpleadoId.Value);
            if (empleado != null)
            {
                empleadoDto = MapToEmpleadoDto(empleado);
            }
        }
        
        return new AuthResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            TokenExpires = DateTime.UtcNow.AddMinutes(120),
            RefreshTokenExpires = DateTime.UtcNow.AddDays(7),
            Empleado = empleadoDto
        };
    }
    
    public async Task<bool> LogoutAsync(string email)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email);
        if (usuario == null)
            return false;
        
        usuario.RefreshToken = null;
        usuario.RefreshTokenExpire = null;
        await _usuarioRepository.UpdateAsync(usuario);
        
        return true;
    }
    
    private string GenerateJwtToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.RoleId.ToString())
        };
        
        // Agregar claim de empleadoId si existe
        if (usuario.EmpleadoId.HasValue)
        {
            claims.Add(new Claim("empleadoId", usuario.EmpleadoId.Value.ToString()));
        }
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(120),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // IMPORTANTE: No validar expiración aquí
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token inválido");
        
        return principal;
    }
    
    private EmpleadoDto MapToEmpleadoDto(Empleado empleado)
    {
        return new EmpleadoDto
        {
            Id = empleado.Id,
            Documento = empleado.Documento,
            Nombres = empleado.Nombres,
            Apellidos = empleado.Apellidos,
            FechaNacimiento = empleado.FechaNacimiento,
            Direccion = empleado.Direccion,
            Telefono = empleado.Telefono,
            Email = empleado.Email,
            Cargo = empleado.Cargo,
            Salario = empleado.Salario,
            FechaIngreso = empleado.FechaIngreso,
            Estado = empleado.Estado,
            NivelEducativo = empleado.NivelEducativo,
            PerfilProfesional = empleado.PerfilProfesional,
            DepartamentoId = empleado.DepartamentoId,
            DepartamentoNombre = empleado.Departamento?.Nombre ?? "No asignado",
            TieneUsuario = empleado.UsuarioId.HasValue
        };
    }
}