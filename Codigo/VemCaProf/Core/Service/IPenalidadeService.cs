using Core.DTO;
using System.Collections.Generic;


namespace Core.Service
{
    public interface IPenalidadeService
    {
        int Create(Penalidade penalidade);
        void Delete(int id);
        void Edit(Penalidade penalidade);

        IEnumerable<Penalidade> GetAll();
        Penalidade Get(int id);
        IEnumerable<Penalidade> GetByPessoaId(int pessoaId);
    }
}
