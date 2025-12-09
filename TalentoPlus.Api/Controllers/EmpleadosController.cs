using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly EmpleadoService _empleadoService;
    
    public EmpleadosController(EmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<EmpleadoDto>> GetMyInfo()
    {
        var empleadoId = User.FindFirstValue("empleadoId");
        if (string.IsNullOrEmpty(empleadoId))
            return Unauthorized();
            
        if (!int.TryParse(empleadoId, out int id))
            return BadRequest("ID de empleado inválido");
            
        var empleado = await _empleadoService.GetByIdAsync(id);
        if (empleado == null)
            return NotFound();
            
        return Ok(empleado);
    }
    
    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<EmpleadoDto>> Registro([FromBody] CreateEmpleadoDto dto)
    {
        try
        {
            // Auto-registro siempre es Empleado
            dto.Role = "Empleado";
            
            var empleado = await _empleadoService.CreateAsync(dto);
            
            // TODO: Enviar email de bienvenida
            
            return CreatedAtAction(nameof(GetMyInfo), new { id = empleado.Id }, empleado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}