using System.Collections.Generic;
using Core.DTO;

namespace Core.Service
{
    public interface IDisponibilidadeHorarioService
    {
        IEnumerable<DisponibilidadeHorarioDTO> GetAll();
        DisponibilidadeHorarioDTO? Get(int id);
     
        int Create(DisponibilidadeHorarioDTO disponibilidadeHorario);
        bool Update(DisponibilidadeHorarioDTO disponibilidadeHorario);
        bool Delete(int id);
    }
}
