using AutoMapper;
using Core;
using VemCaProfAPI.Models;
using System.Linq;
using System.Collections.Generic;

namespace VemCaProfAPI.Mappers
{
    public class PessoaApiProfile : Profile
    {
        public PessoaApiProfile()
        {
            // Mapeamento de Pessoa (entidade) para PessoaApiModel
            CreateMap<Pessoa, PessoaApiModel>()
                .ForMember(dest => dest.IdDisciplinas, opt => opt.MapFrom(src =>
                    src.IdDisciplinas != null
                        ? src.IdDisciplinas.Select(d => (int)d.Id).ToList()
                        : new List<int>()));

            // Mapeamento de PessoaApiModel para Pessoa (entidade)
            CreateMap<PessoaApiModel, Pessoa>()
                .ForMember(dest => dest.IdDisciplinas, opt => opt.MapFrom(src =>
                    src.IdDisciplinas != null
                        ? src.IdDisciplinas.Select(id => new Disciplina { Id = (uint)id }).ToList()
                        : new List<Disciplina>()))
                .ForMember(dest => dest.InverseResponsavel, opt => opt.Ignore())
                .ForMember(dest => dest.IdCidadeNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.AulaIdAlunoNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.AulaIdProfessorNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.AulaIdResponsavelNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.PenalidadeIdProfessorNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.PenalidadeIdResponsavelNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.DisponibilidadeHorarios, opt => opt.Ignore());
        }
    }
}