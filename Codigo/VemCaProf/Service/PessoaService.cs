using Core;
using Core.DTO;
using Core.Mappers; 
using Core.Service;
using Microsoft.EntityFrameworkCore;


namespace Service
{
    internal class PessoaService : IPessoaService
    {
        private readonly VemCaProfContext _context;

        public PessoaService(VemCaProfContext context)
        {
            _context = context;
        }

        // PROFESSOR

        /// <summary>
        /// Criar um novo professor na base de dados
        /// </summary>
        /// <param name="dto">DTO com dados do professor</param>
        /// <returns>id do professor criado</returns>
        public void CreateProfessor(ProfessorPessoaDTO dto)
        {
            var pessoa = PessoaProfile.ToEntity(dto);

            pessoa.Senha = dto.Senha;

            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();
            
        }

        /// <summary>
        /// Editar dados do professor
        /// </summary>
        /// <param name="dto">DTO com dados atualizados</param>
        /// <exception cref="ServiceException">lançada quando não encontrado</exception>
        public void EditProfessor(ProfessorPessoaDTO dto)
        {
            if (dto.Id == 0)
                throw new ServiceException("ID do professor inválido.");

            var pessoaExistente = _context.Pessoas.Find(dto.Id);
            if (pessoaExistente == null)
                throw new ServiceException("Professor não encontrado.");

            // Atualização manual simples
            pessoaExistente.Nome = dto.Nome;
            pessoaExistente.Sobrenome = dto.Sobrenome;
            pessoaExistente.Email = dto.Email;
            pessoaExistente.Telefone = dto.Telefone;
            
            // Dados específicos
            pessoaExistente.DescricaoProfessor = dto.DescricaoProfessor;
            pessoaExistente.Libras = dto.Libras;
            pessoaExistente.IdCidade = dto.IdCidade;

            // Arquivos (só atualiza se vier novo)
            if (dto.Diploma != null) pessoaExistente.Diploma = dto.Diploma;
            if (dto.FotoPerfil != null) pessoaExistente.FotoPerfil = dto.FotoPerfil;

            // Senha (só altera se o usuário digitou algo novo)
            if (!string.IsNullOrEmpty(dto.Senha))
            {
                pessoaExistente.Senha = dto.Senha;
            }

            _context.Update(pessoaExistente);
            _context.SaveChanges();
        }

        /// <summary>
        /// Buscar professor pelo id
        /// </summary>
        /// <param name="id">id do professor</param>
        /// <returns>Entidade Pessoa (Professor)</returns>
        public Pessoa GetProfessor(int id)
        {
            return _context.Pessoas.FirstOrDefault(p => p.Id == id) 
                ?? throw new ServiceException("Professor não encontrado.");
        }

        /// <summary>
        /// Buscar todos os professores
        /// </summary>
        /// <returns>lista de professores</returns>
        public IEnumerable<Pessoa> GetAllProfessores()
        {
            // Filtra onde DescricaoProfessor não é nulo (indica que é professor)
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.DescricaoProfessor != null);
        }

        /// <summary>
        /// Obtém professores pelo nome
        /// </summary>
        /// <param name="nome">parte do nome</param>
        /// <returns>lista de professores filtrada</returns>
        public IEnumerable<Pessoa> GetProfessoresByNome(string nome)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.DescricaoProfessor != null && p.Nome.StartsWith(nome));
        }

        // ALUNO

        /// <summary>
        /// Criar um novo aluno na base de dados
        /// </summary>
        /// <param name="dto">DTO com dados do aluno</param>
        public void CreateAluno(AlunoPessoaDTO dto)
        {
            var pessoa = PessoaProfile.ToEntity(dto);
            pessoa.Senha = dto.Senha;

            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();
        }

        /// <summary>
        /// Editar dados do aluno
        /// </summary>
        /// <param name="dto">DTO com dados do aluno</param>
        public void EditAluno(AlunoPessoaDTO dto)
        {
            if (dto.Id == 0) throw new ServiceException("ID inválido.");

            var pessoaExistente = _context.Pessoas.Find(dto.Id);
            if (pessoaExistente == null) throw new ServiceException("Aluno não encontrado.");

            // Atualização manual simples
            pessoaExistente.Nome = dto.Nome;
            pessoaExistente.Sobrenome = dto.Sobrenome;
            pessoaExistente.AlunoDeMenor = dto.AlunoDeMenor;
            pessoaExistente.Atipico = dto.Atipico;

            if (!string.IsNullOrEmpty(dto.Senha))
            {
                pessoaExistente.Senha = dto.Senha;
            }

            _context.Update(pessoaExistente);
            _context.SaveChanges();
        }

        public Pessoa GetAluno(int id)
        {
            return _context.Pessoas.FirstOrDefault(p => p.Id == id)
                ?? throw new ServiceException("Aluno não encontrado.");
        }

        public IEnumerable<Pessoa> GetAllAlunos()
        {
            // Filtra onde AlunoDeMenor tem valor (indica que é aluno)
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.AlunoDeMenor != null);
        }

        public IEnumerable<Pessoa> GetAlunosByNome(string nome)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.AlunoDeMenor != null && p.Nome.StartsWith(nome));
        }

        // RESPONSÁVEL

        public void CreateResponsavel(ResponsavelPessoaDTO dto)
        {
            var pessoa = PessoaProfile.ToEntity(dto);
            pessoa.Senha = dto.Senha;

            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();
        }

        public void EditResponsavel(ResponsavelPessoaDTO dto)
        {
            var pessoa = _context.Pessoas.Find(dto.Id) 
                         ?? throw new ServiceException("Responsável não encontrado.");
            
            pessoa.Nome = dto.Nome;
            pessoa.QuantidadeDeDependentes = dto.QuantidadeDeDependentes;
            
            if(!string.IsNullOrEmpty(dto.Senha))
                pessoa.Senha = dto.Senha;

            _context.Update(pessoa);
            _context.SaveChanges();
        }

        public Pessoa GetResponsavel(int id)
        {
             return _context.Pessoas.Find(id) 
                    ?? throw new ServiceException("Responsável não encontrado.");
        }

        public IEnumerable<Pessoa> GetAllResponsaveis()
        {
            return _context.Pessoas.AsNoTracking()
                .Where(p => p.QuantidadeDeDependentes != null);
        }

        public IEnumerable<Pessoa> GetResponsaveisByNome(string nome)
        {
            return _context.Pessoas.AsNoTracking()
                .Where(p => p.QuantidadeDeDependentes != null && p.Nome.StartsWith(nome));
        }

        // DELETE GENERICO PARA TODOS
    

        /// <summary>
        /// Remover pessoa (professor, aluno ou responsável) da base de dados
        /// </summary>
        /// <param name="id">id da Pessoa</param>
        
        public void Delete(int id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if (pessoa != null)
            {
                _context.Remove(pessoa);
                _context.SaveChanges();
            }
        }
    }
}