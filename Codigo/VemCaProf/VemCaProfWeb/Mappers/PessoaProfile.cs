using AutoMapper;
using Core;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Mappers
{
    public class PessoaProfile : Profile
    {
        public PessoaProfile()
        {
            // Mapeamento PessoaModel -> Pessoa
            CreateMap<PessoaModel, Pessoa>()
                .ForMember(dest => dest.InverseResponsavel, opt => opt.MapFrom(src => src.Dependentes))
                .ForMember(dest => dest.IdDisciplinas, opt => opt.MapFrom(src =>
                    src.IdDisciplinas == null
                        ? new List<Disciplina>()
                        : src.IdDisciplinas.Select(id => new Disciplina { Id = (uint) id }).ToList()))
                .ForMember(dest => dest.Diploma, opt => opt.Ignore())
                .ForMember(dest => dest.FotoPerfil, opt => opt.Ignore())
                .ForMember(dest => dest.FotoDocumento, opt => opt.Ignore());

            // Mapeamento reverso Pessoa -> PessoaModel
            CreateMap<Pessoa, PessoaModel>()
                .ForMember(dest => dest.Dependentes, opt => opt.MapFrom(src => src.InverseResponsavel))
                .ForMember(dest => dest.IdDisciplinas, opt => opt.MapFrom(src =>
                    src.IdDisciplinas == null
                        ? new List<int>()
                        : src.IdDisciplinas.Select(d => (int)d.Id).ToList()))
                .ForMember(dest => dest.FotoPerfil, opt => opt.MapFrom(src => src.FotoPerfil))
                .ForMember(dest => dest.Diploma, opt => opt.MapFrom(src => src.Diploma))
                .ForMember(dest => dest.FotoDocumento, opt => opt.MapFrom(src => src.FotoDocumento));
        }
    }
}