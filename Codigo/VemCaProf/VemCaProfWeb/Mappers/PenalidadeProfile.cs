using AutoMapper;
using Core;
using VemCaProfWeb.Models;

namespace Mappers
{
    public class PenalidadeProfile : Profile
    {
        public PenalidadeProfile()
        {
            CreateMap<PenalidadeModel, Penalidade>().ReverseMap();
        }
    }
}
