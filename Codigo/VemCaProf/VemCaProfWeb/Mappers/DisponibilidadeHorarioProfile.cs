using AutoMapper;
using Core.DTO;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Mappers
{
    public class DisponibilidadeHorarioProfile : Profile
    {
        public DisponibilidadeHorarioProfile()
        {
            CreateMap<DisponibilidadeHorarioDTO, DisponibilidadeHorarioModel>().ReverseMap();
        }
    }
}