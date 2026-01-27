using AutoMapper;
using Core;
using Core.DTO;


namespace Core.Mappers
{
    public class PenalidadeMapper : Profile
    {
        public PenalidadeMapper()
        {
            CreateMap<Penalidade, PenalidadeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DataHorarioInicio, opt => opt.MapFrom(src => src.DataHorarioInicio))
                .ForMember(dest => dest.DataHoraFim, opt => opt.MapFrom(src => src.DataHoraFim))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo))
                .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao));
        }
    }
}
