using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Core.Service
{
    public interface IPessoaService
    {
        int Create(Pessoa pessoa);

        void Edit(Pessoa pessoa);

        void Delete(int id);

        // --- MÉTODOS DE BUSCA ---

        Pessoa Get(int id);
        
        Pessoa GetByCpf(string cpf);
        
        IEnumerable<Pessoa> GetAll();
        
        IEnumerable<Pessoa> GetByNome(string nome);

        IEnumerable<Pessoa> GetAllProfessores();

        IEnumerable<Pessoa> GetAllResponsaveis();
    }
}