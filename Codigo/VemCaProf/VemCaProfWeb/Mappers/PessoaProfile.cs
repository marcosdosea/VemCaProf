using AutoMapper;
using Core;          // <--- Adicione isso para enxergar a classe 'Pessoa'
using Core.DTO;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Mappers // Recomendo usar o namespace completo do projeto
{
    public class PessoaProfile : Profile
    {
        public PessoaProfile()
        {
            // ==========================================================
            // 1. LEITURA (Do Banco para a Tela)
            // Entity (Pessoa) -> ViewModel
            // ==========================================================
            
            // Resolve o erro: Missing map configuration Pessoa -> ResponsavelPessoaModel
            CreateMap<Pessoa, ResponsavelPessoaModel>();
            
            CreateMap<Pessoa, AlunoPessoaModel>()
                // Se o nome da propriedade for diferente (ex: ResponsavelId vs IdResponsavel), precisa explicar:
                .ForMember(dest => dest.IdResponsavel, opt => opt.MapFrom(src => src.ResponsavelId));

            CreateMap<Pessoa, ProfessorPessoaModel>();


            // ==========================================================
            // 2. ESCRITA (Da Tela para o Serviço)
            // ViewModel -> DTO
            // ==========================================================
            
            CreateMap<ProfessorPessoaModel, ProfessorPessoaDTO>().ReverseMap();
            CreateMap<AlunoPessoaModel, AlunoPessoaDTO>().ReverseMap();
            CreateMap<ResponsavelPessoaModel, ResponsavelPessoaDTO>().ReverseMap();
        }
    }
}