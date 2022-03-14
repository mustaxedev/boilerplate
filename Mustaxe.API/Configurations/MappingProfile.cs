using AutoMapper;
using Mustaxe.Application.ViewModels.CEP;
using Mustaxe.Domain.Entities;
using Mustaxe.Integration.ViaCEP.Responses;
using System.Diagnostics.CodeAnalysis;

namespace Mustaxe.API.Configurations
{
    [ExcludeFromCodeCoverage]
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<ConsultaCEPResponse, ConsultarCEPViewModel>();
            CreateMap<ConsultaCEP, ConsultarCEPViewModel>();
            CreateMap<ConsultaCEPResponse, ConsultaCEP>()
                .ForMember(dest => dest.CEP, opt => opt.MapFrom(src => src.Cep.Replace("-", "")));
        }
    }
}
