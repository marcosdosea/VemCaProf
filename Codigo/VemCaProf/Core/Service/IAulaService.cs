using System.Collections.Generic;
using Core.DTO;

namespace Core.Service
{
    public interface IAulaService
    {
        IEnumerable<AulaDTO> GetAll();
        AulaDTO? Get(int id);
        IEnumerable<DisponibilidadeHorarioDTO> GetHorariosDisponiveis(int idProfessor, DateTime dataAula, int? idAula = null);
        int Create(AulaDTO aula);
        bool Update(AulaDTO aula);
        bool Delete(int id);
        void CancelarAula(int id);
        void ConfirmarAula(int id);
    }
}
