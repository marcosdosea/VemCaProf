using AutoMapper;
using Core.DTO;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Mappers
{
    public class CidadeProfile : Profile
    {
        public CidadeProfile()
        {
            CreateMap<CidadeDTO, CidadeModel>().ReverseMap();
        }
    }
}