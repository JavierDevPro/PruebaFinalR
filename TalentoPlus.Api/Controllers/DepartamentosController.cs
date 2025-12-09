using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : ControllerBase
{
    private readonly IDepartamentoService _departamentoService;
    
    public DepartamentosController(IDepartamentoService departamentoService)
    {
        _departamentoService = departamentoService;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetDepartamentos()
    {
        try
        {
            var departamentos = await _departamentoService.GetAllDepartamentosAsync();
            return Ok(departamentos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Error al obtener departamentos");
        }
    }
}