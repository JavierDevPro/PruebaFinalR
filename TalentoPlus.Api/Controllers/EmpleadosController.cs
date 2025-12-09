using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;
    
    public EmpleadosController(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }
    
    [HttpGet]
    [Authorize(Roles = "1")] // Solo Admin (RoleId = 1)
    public async Task<ActionResult<IEnumerable<EmpleadoDto>>> GetAll()
    {
        try
        {
            var empleados = await _empleadoService.GetAllEmpleadosAsync();
            return Ok(empleados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<EmpleadoDto>> GetById(int id)
    {
        try
        {
            // Verificar si el usuario puede ver este empleado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var empleadoId = User.FindFirstValue("empleadoId");
            
            // Admin puede ver cualquier empleado
            if (userRole != "1") // Si no es Admin
            {
                if (!int.TryParse(empleadoId, out int empId) || empId != id)
                    return Forbid();
            }
            
            var empleado = await _empleadoService.GetEmpleadoByIdAsync(id);
            if (empleado == null)
                return NotFound();
                
            return Ok(empleado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<EmpleadoDto>> GetMyInfo()
    {
        try
        {
            var empleadoId = User.FindFirstValue("empleadoId");
            if (string.IsNullOrEmpty(empleadoId) || !int.TryParse(empleadoId, out int id))
                return Unauthorized();
                
            var empleado = await _empleadoService.GetEmpleadoByIdAsync(id);
            if (empleado == null)
                return NotFound();
                
            return Ok(empleado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("me/pdf")]
    public async Task<IActionResult> DownloadMyPdf()
    {
        try
        {
            var empleadoId = User.FindFirstValue("empleadoId");
            if (string.IsNullOrEmpty(empleadoId) || !int.TryParse(empleadoId, out int id))
                return Unauthorized();
                
            var pdfBytes = await _empleadoService.GeneratePdfAsync(id);
            
            return File(pdfBytes, "application/pdf", $"hoja-vida-{id}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost]
    [Authorize(Roles = "1")] // Solo Admin
    public async Task<ActionResult<EmpleadoDto>> Create([FromBody] CreateEmpleadoDto dto)
    {
        try
        {
            var empleado = await _empleadoService.CreateEmpleadoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = empleado.Id }, empleado);
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
    
    [HttpPut("{id}")]
    [Authorize(Roles = "1")] // Solo Admin
    public async Task<ActionResult> Update(int id, [FromBody] UpdateEmpleadoDto dto)
    {
        try
        {
            await _empleadoService.UpdateEmpleadoAsync(id, dto);
            return NoContent();
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
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "1")] // Solo Admin
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _empleadoService.DeleteEmpleadoAsync(id);
            return NoContent();
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
}