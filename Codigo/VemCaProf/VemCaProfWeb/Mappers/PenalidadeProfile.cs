using AutoMapper;
using Core;
using Core.DTO;
using VemCaProfWeb.Models;

namespace Mappers
{
    public class PenalidadeProfile : Profile
    {
        public PenalidadeProfile()
        {
            CreateMap<Penalidade, PenalidadeModel>();
            CreateMap<PenalidadeModel, Penalidade>(); // se precisar mapear de volta
            CreateMap<Penalidade, PenalidadeDTO>();   // se usar DTOs
            CreateMap<PenalidadeDTO, Penalidade>();
            CreateMap<PenalidadeModel, PenalidadeDTO>().ReverseMap();
        }
    }
}
