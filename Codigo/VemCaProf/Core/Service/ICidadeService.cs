using System.Collections.Generic;
using Core.DTO;

namespace Core.Service
{
    public interface ICidadeService
    {
        IEnumerable<CidadeDTO> GetAll();
        CidadeDTO? Get(int id);
        CidadeDTO? GetByNomeEstado(string nome, string estado);
        int Create(CidadeDTO cidade);
        bool Update(CidadeDTO cidade);
        bool Delete(int id);
    }
}