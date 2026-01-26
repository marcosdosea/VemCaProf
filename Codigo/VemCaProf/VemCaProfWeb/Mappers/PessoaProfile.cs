using AutoMapper;
using Core.DTO;
using VemCaProfWeb.Models;

namespace Mappers;

public class PessoaProfile : Profile

{
    public PessoaProfile()

    {
        // PROFESSOR (ViewModel <-> DTO)

        CreateMap<ProfessorPessoaModel, ProfessorPessoaDTO>().ReverseMap();


        // ALUNO (ViewModel <-> DTO)

        CreateMap<AlunoPessoaModel, AlunoPessoaDTO>().ReverseMap();


        // RESPONSÁVEL (ViewModel <-> DTO)

        CreateMap<ResponsavelPessoaModel, ResponsavelPessoaDTO>().ReverseMap();
    }
}