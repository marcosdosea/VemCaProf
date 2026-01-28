using Core.DTO;
using System.Collections.Generic;


namespace Core.Service
{
    public interface IPenalidadeService
    {
        int Create(PenalidadeDTO penalidade);
        void Delete(int id);
        void Edit(PenalidadeDTO penalidade);
        IEnumerable<PenalidadeDTO> GetAll();
        IEnumerable<PenalidadeDTO> GetByPessoaId(int pessoaId);
    }
}
