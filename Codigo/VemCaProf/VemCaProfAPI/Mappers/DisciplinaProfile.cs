using AutoMapper;
using Core;
using VemCaProfWeb.Models;

namespace Mappers
{
    public class DisciplinaProfile : Profile
    {
        public DisciplinaProfile()
        {
            CreateMap<DisciplinaModel, Disciplina>().ReverseMap();
        }
    }
}
