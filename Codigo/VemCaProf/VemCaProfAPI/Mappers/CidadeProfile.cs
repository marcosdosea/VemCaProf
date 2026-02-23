using AutoMapper;
using Core.DTO;
using VemCaProfAPI.Models;

namespace VemCaProfAPI.Mappers
{
    public class CidadeProfile : Profile
    {
        public CidadeProfile()
        {
            CreateMap<CidadeModel, CidadeDTO>().ReverseMap();
        }
    }
}