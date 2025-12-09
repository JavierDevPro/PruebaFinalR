namespace TalentoPlus.Application.Interfaces;

public interface IDepartamentoService
{
    Task<IEnumerable<object>> GetAllDepartamentosAsync();
}