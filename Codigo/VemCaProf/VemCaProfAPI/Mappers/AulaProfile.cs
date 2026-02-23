using AutoMapper;
using Core.DTO;
using VemCaProfAPI.Models;

namespace VemCaProfAPI.Mappers
{
    public class AulaProfile : Profile
    {
        public AulaProfile()
        {
            CreateMap<AulaDTO, AulaModel>().ReverseMap();
        }
    }
}
