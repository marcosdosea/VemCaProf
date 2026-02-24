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

        /// <summary>
        /// Metodos de Busca
        /// </summary>
        
        Pessoa Get(int id);
        
        Pessoa? GetByCpf(string? cpf);
        
        Pessoa? GetByEmail(string email);
        
        IEnumerable<Pessoa> GetAll();
        
        IEnumerable<Pessoa> GetByNome(string nome);

        IEnumerable<Pessoa> GetAllProfessores();

        IEnumerable<Pessoa> GetAllResponsaveis();
        
        IEnumerable<Pessoa> GetListaParaIndex(string? tipo, string? cpfLogado, bool isAdmin);
        
        Pessoa? GetModelParaCreate(string tipo, string cpfLogado, string emailLogado, bool isAdmin, bool isProfessor, bool isResponsavel);
        
        void CreateSeguro(Pessoa pessoa, string? cpfLogado, bool isAdmin, bool isProfessor, bool isResponsavel);
        
        Pessoa? GetParaDetails(int id, string? cpfLogado, bool isAdmin);
        
        Pessoa? GetParaEdit(int id, string? cpfLogado, bool isAdmin);
        
        bool EditSeguro(Pessoa pessoa, string? cpfLogado, bool isAdmin);
        
        Pessoa? GetParaDelete(int id, string? cpfLogado, bool isAdmin);
        
        bool DeleteSeguro(int id, string? cpfLogado, bool isAdmin);
        
        bool ResponsavelPrecisaCadastrarAlunos(string? cpfLogado);
        
        public void AtualizarEmailPessoa(int pessoaId, string novoEmail);
    }
    
}