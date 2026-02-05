using AutoMapper;
using Core;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Mappers
{
    public class PessoaProfile : Profile
    {
        public PessoaProfile()
        {
            CreateMap<PessoaModel, Pessoa>()
                
                .ForMember(dest => dest.IdDisciplinas, opt => opt.MapFrom(src => 
                    src.DisciplinasSelecionadas != null 
                        ? src.DisciplinasSelecionadas.Select(id => new Disciplina { Id = id }).ToList() 
                        : new List<Disciplina>()))
                
                // Ignora campos de arquivo 
                .ForMember(dest => dest.Diploma, opt => opt.Ignore())
                .ForMember(dest => dest.FotoPerfil, opt => opt.Ignore())
                .ForMember(dest => dest.FotoDocumento, opt => opt.Ignore())

                .ReverseMap()
                
                
                .ForMember(dest => dest.DisciplinasSelecionadas, opt => opt.MapFrom(src => 
                    src.IdDisciplinas.Select(d => d.Id).ToList()));
        }
    }
}