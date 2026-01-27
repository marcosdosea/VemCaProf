// Core/Mappers/CidadeProfile.cs
using AutoMapper;
using Core;
using Core.DTO;

namespace Core.Mappers
{
    public class CidadeMapper : Profile
    {
        public CidadeMapper()
        {
            // Mapeamento Cidade (Entity) <-> CidadeDTO
            CreateMap<Cidade, CidadeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado));

            CreateMap<CidadeDTO, Cidade>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado));
        }
    }
}