using AutoMapper;
using Core.DTO;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Mappers
{
    public class AulaProfile : Profile
    {
        public AulaProfile()
        {
            CreateMap<AulaDTO, AulaModel>().ReverseMap();
        }
    }
}