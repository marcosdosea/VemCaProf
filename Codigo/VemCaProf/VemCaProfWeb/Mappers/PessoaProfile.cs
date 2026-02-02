using AutoMapper;
using Core;          
using Core.DTO;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Mappers 
{
    public class PessoaProfile : Profile
    {
        public PessoaProfile()
        {

            CreateMap<Pessoa, ResponsavelPessoaModel>();

            CreateMap<Pessoa, AlunoPessoaModel>();

            CreateMap<Pessoa, ProfessorPessoaModel>();
            
            CreateMap<ProfessorPessoaModel, ProfessorPessoaDTO>().ReverseMap();
            
            CreateMap<AlunoPessoaModel, AlunoPessoaDTO>().ReverseMap();
            
            CreateMap<ResponsavelPessoaModel, ResponsavelPessoaDTO>().ReverseMap();
        }
    }
}