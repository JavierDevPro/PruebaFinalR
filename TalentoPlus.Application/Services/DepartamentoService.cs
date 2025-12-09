using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services;

public class DepartamentoService : IDepartamentoService
{
    private readonly IDepartamentoRepository _departamentoRepository;
    
    public DepartamentoService(IDepartamentoRepository departamentoRepository)
    {
        _departamentoRepository = departamentoRepository;
    }
    
    public async Task<IEnumerable<object>> GetAllDepartamentosAsync()
    {
        var departamentos = await _departamentoRepository.GetAllAsync();
        return departamentos.Select(d => new 
        { 
            d.Id, 
            d.Nombre, 
            d.Descripcion 
        });
    }
}