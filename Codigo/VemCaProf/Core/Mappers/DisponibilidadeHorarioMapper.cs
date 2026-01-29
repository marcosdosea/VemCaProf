
using AutoMapper;
using Core;
using Core.DTO;

namespace Core.Mappers
{
    public class DisponibilidadeHorarioMapper : Profile
    {
        public DisponibilidadeHorarioMapper()
        {
            // Mapeamento Horário (Entity) <-> DisponibilidadeHorarioDTO
            CreateMap<DisponibilidadeHorario, DisponibilidadeHorarioDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Dia, opt => opt.MapFrom(src => src.Dia))
                .ForMember(dest => dest.HorarioInicio, opt => opt.MapFrom(src => src.HorarioInicio))
                .ForMember(dest => dest.HorarioFim, opt => opt.MapFrom(src => src.HorarioFim))
                .ForMember(dest => dest.IdProfessor, opt => opt.MapFrom(src => src.IdProfessor));

            CreateMap<DisponibilidadeHorarioDTO, DisponibilidadeHorario>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Dia, opt => opt.MapFrom(src => src.Dia))
                .ForMember(dest => dest.HorarioInicio, opt => opt.MapFrom(src => src.HorarioInicio))
                .ForMember(dest => dest.HorarioFim, opt => opt.MapFrom(src => src.HorarioFim))
                .ForMember(dest => dest.IdProfessor, opt => opt.MapFrom(src => src.IdProfessor));
        }
    }
}
