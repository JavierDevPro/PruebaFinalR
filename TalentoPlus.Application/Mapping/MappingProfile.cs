using AutoMapper;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Empleado, EmpleadoDto>()
            .ForMember(dest => dest.DepartamentoNombre, 
                opt => opt.MapFrom(src => src.Departamento != null ? src.Departamento.Nombre : ""));
        
        CreateMap<CreateEmpleadoDto, Empleado>();
        CreateMap<UpdateEmpleadoDto, Empleado>();
    }
}