using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PessoaService : IPessoaService
    {
        private const string TipoProfessor = "P";
        private const string TipoAluno = "A";
        private const string TipoResponsavel = "R";

        private readonly VemCaProfContext _context;

        public PessoaService(VemCaProfContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria uma nova pessoa no banco de dados.
        /// </summary>
        /// <param name="pessoa">Objeto pessoa a ser criado</param>
        /// <returns>ID da pessoa criada</returns>
        /// <exception cref="ServiceException">Lançada quando CPF ou e-mail já existem, ou dados específicos inválidos</exception>
        public int Create(Pessoa pessoa)
        {
            ValidarCpfUnico(pessoa.Cpf);
            ValidarEmailUnico(pessoa.Email, pessoa.TipoPessoa);
            ValidarDadosEspecificos(pessoa);

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (pessoa.TipoPessoa == TipoAluno)
                {
                    var responsavel = _context.Pessoas
                        .FirstOrDefault(p => p.Id == pessoa.ResponsavelId && p.TipoPessoa == TipoResponsavel);

                    if (responsavel == null)
                        throw new ServiceException("Responsável não encontrado.");

                    int dependentesAtuais = _context.Pessoas
                        .Count(p => p.ResponsavelId == pessoa.ResponsavelId && p.TipoPessoa == TipoAluno);

                    var limite = responsavel.QuantidadeDeDependentes ?? 0;
                    if (dependentesAtuais >= limite)
                        throw new ServiceException($"Limite de dependentes atingido ({limite}). Aumente o limite no perfil do responsável.");
                }

                _context.Pessoas.Add(pessoa);
                _context.SaveChanges();
                transaction.Commit();
                return pessoa.Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Cria uma nova pessoa com validações de segurança baseadas no perfil do usuário logado.
        /// </summary>
        /// <param name="pessoa">Objeto pessoa a ser criado</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <param name="isProfessor">Indica se o usuário logado tem perfil de professor</param>
        /// <param name="isResponsavel">Indica se o usuário logado tem perfil de responsável</param>
        /// <exception cref="ServiceException">Lançada quando o usuário não tem permissão para criar o tipo de pessoa</exception>
        public void CreateSeguro(Pessoa pessoa, string? cpfLogado, bool isAdmin, bool isProfessor, bool isResponsavel)
        {
            if (!isAdmin)
            {
                bool permitido = pessoa.TipoPessoa switch
                {
                    TipoProfessor => isProfessor,
                    TipoResponsavel => isResponsavel,
                    TipoAluno => isResponsavel,
                    _ => false
                };

                if (!permitido)
                    throw new ServiceException("Você não tem permissão para criar este tipo de perfil.");
            }

            if (pessoa.TipoPessoa == TipoAluno && !isAdmin)
            {
                var responsavel = GetByCpf(cpfLogado);
                pessoa.ResponsavelId = responsavel?.Id;
            }
            else if (!isAdmin)
            {
                pessoa.Cpf = cpfLogado;
            }

            if (pessoa.TipoPessoa == TipoResponsavel && pessoa.InverseResponsavel.Any())
            {
                foreach (var filho in pessoa.InverseResponsavel)
                {
                    filho.TipoPessoa = TipoAluno;
                }
            }

            Create(pessoa);
        }

        /// <summary>
        /// Edita os dados de uma pessoa existente.
        /// </summary>
        /// <param name="pessoaAtualizada">Objeto com os novos dados da pessoa</param>
        /// <exception cref="ServiceException">Lançada quando a pessoa não é encontrada</exception>
        public void Edit(Pessoa pessoaAtualizada)
        {
            var pessoaOriginal = _context.Pessoas
                .Include(p => p.IdDisciplinas)
                .FirstOrDefault(p => p.Id == pessoaAtualizada.Id)
                ?? throw new ServiceException("Pessoa não encontrada.");

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                AtualizarDadosComuns(pessoaOriginal, pessoaAtualizada);
                AtualizarArquivos(pessoaOriginal, pessoaAtualizada);
                AtualizarDadosEspecificos(pessoaOriginal, pessoaAtualizada);

                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Edita uma pessoa verificando permissões do usuário logado.
        /// </summary>
        /// <param name="pessoaDaTela">Objeto com os novos dados da pessoa</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <returns>True se a edição foi bem‑sucedida, False se não tiver permissão</returns>
        public bool EditSeguro(Pessoa pessoaDaTela, string? cpfLogado, bool isAdmin)
        {
            if (!TemPermissao(pessoaDaTela.Id, cpfLogado, isAdmin, out var entity))
                return false;

            Edit(pessoaDaTela);
            return true;
        }

        /// <summary>
        /// Remove (exclui logicamente) uma pessoa, impedindo exclusão de responsável com dependentes.
        /// </summary>
        /// <param name="id">ID da pessoa a ser excluída</param>
        /// <exception cref="ServiceException">Lançada quando se tenta excluir um responsável que possui dependentes</exception>
        public void Delete(int id)
        {
            var pessoa = _context.Pessoas
                .Include(p => p.InverseResponsavel)
                .FirstOrDefault(p => p.Id == id);

            if (pessoa == null) return;

            if (pessoa.TipoPessoa == TipoResponsavel && pessoa.InverseResponsavel.Any())
            {
                throw new ServiceException("Não é possível excluir um responsável com dependentes.");
            }

            if (_context.Penalidades.Any(p => p.IdResponsavel == id))
            {
                throw new ServiceException("Não é possível excluir esta pessoa, pois existem penalidades vinculadas a ela.");
            }

            try
            {
                _context.Pessoas.Remove(pessoa);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _context.Entry(pessoa).State = EntityState.Unchanged;
                throw new ServiceException(
                    "Não é possível excluir esta pessoa, pois existem outros registros no sistema vinculados a ela.", ex);
            }
        }

        /// <summary>
        /// Remove uma pessoa verificando permissões do usuário logado.
        /// </summary>
        /// <param name="id">ID da pessoa a ser excluída</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <returns>True se a exclusão foi bem‑sucedida, False se não tiver permissão</returns>
        public bool DeleteSeguro(int id, string? cpfLogado, bool isAdmin)
        {
            if (!TemPermissao(id, cpfLogado, isAdmin, out _))
                return false;

            Delete(id);
            return true;
        }

        /// <summary>
        /// Obtém uma pessoa pelo ID.
        /// </summary>
        /// <param name="id">ID da pessoa</param>
        /// <returns>Objeto pessoa ou null se não encontrado</returns>
        public Pessoa? Get(int id)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Include(p => p.IdDisciplinas)
                .Include(p => p.InverseResponsavel)
                .FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Obtém uma pessoa para exibição de detalhes, verificando permissão.
        /// </summary>
        /// <param name="id">ID da pessoa</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <returns>Objeto pessoa se permitido, caso contrário null</returns>
        public Pessoa? GetParaDetails(int id, string? cpfLogado, bool isAdmin)
        {
            var entity = Get(id);
            return PodeVer(entity, cpfLogado, isAdmin) ? entity : null;
        }

        /// <summary>
        /// Obtém uma pessoa para edição, verificando permissão.
        /// </summary>
        /// <param name="id">ID da pessoa</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <returns>Objeto pessoa se permitido, caso contrário null</returns>
        public Pessoa? GetParaEdit(int id, string? cpfLogado, bool isAdmin)
        {
            if (!TemPermissao(id, cpfLogado, isAdmin, out var entity))
                return null;
            return entity;
        }

        /// <summary>
        /// Obtém uma pessoa para exclusão, verificando permissão.
        /// </summary>
        /// <param name="id">ID da pessoa</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <returns>Objeto pessoa se permitido, caso contrário null</returns>
        public Pessoa? GetParaDelete(int id, string? cpfLogado, bool isAdmin)
        {
            if (!TemPermissao(id, cpfLogado, isAdmin, out var entity))
                return null;
            return entity;
        }

        /// <summary>
        /// Retorna a lista de pessoas para a view Index, filtrada conforme o perfil do usuário.
        /// </summary>
        /// <param name="tipo">Filtro por tipo de pessoa (opcional)</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <returns>Lista de pessoas</returns>
        public IEnumerable<Pessoa> GetListaParaIndex(string? tipo, string? cpfLogado, bool isAdmin)
        {
            var query = _context.Pessoas.AsNoTracking();

            if (isAdmin)
            {
                if (!string.IsNullOrEmpty(tipo))
                    query = query.Where(p => p.TipoPessoa == tipo);
                return query.ToList();
            }

            var usuarioRaiz = GetByCpf(cpfLogado);
            if (usuarioRaiz == null)
                return new List<Pessoa>();

            if (usuarioRaiz.TipoPessoa == TipoResponsavel)
            {
                return query.Where(p => p.Id == usuarioRaiz.Id || p.ResponsavelId == usuarioRaiz.Id).ToList();
            }

            return new List<Pessoa> { usuarioRaiz };
        }

        /// <summary>
        /// Prepara o modelo para a tela de criação, verificando permissões e se já existe perfil.
        /// </summary>
        /// <param name="tipo">Tipo de pessoa a ser criada</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="emailLogado">E‑mail do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <param name="isProfessor">Indica se o usuário logado tem perfil de professor</param>
        /// <param name="isResponsavel">Indica se o usuário logado tem perfil de responsável</param>
        /// <returns>Entidade pessoa (nova ou existente) ou null se não permitido</returns>
        public Pessoa? GetModelParaCreate(string tipo, string cpfLogado, string emailLogado, bool isAdmin, bool isProfessor, bool isResponsavel)
        {
            if (string.IsNullOrEmpty(tipo) || (tipo != TipoProfessor && tipo != TipoAluno && tipo != TipoResponsavel))
                return null;

            bool permitido = tipo switch
            {
                TipoProfessor => isAdmin || isProfessor,
                TipoAluno => isAdmin || isResponsavel,
                TipoResponsavel => isAdmin || isResponsavel,
                _ => false
            };

            if (!permitido)
                return null;

            var existente = GetByCpf(cpfLogado);
            if (existente != null && tipo != TipoAluno)
                return existente;

            if (tipo == TipoAluno)
            {
                var responsavel = GetByCpf(cpfLogado);
                return new Pessoa { TipoPessoa = TipoAluno, ResponsavelId = responsavel?.Id };
            }

            return new Pessoa { TipoPessoa = tipo, Cpf = cpfLogado, Email = emailLogado };
        }

        /// <summary>
        /// Busca uma pessoa pelo CPF.
        /// </summary>
        /// <param name="cpf">CPF da pessoa</param>
        /// <returns>Objeto pessoa ou null</returns>
        public Pessoa? GetByCpf(string? cpf)
        {
            if (string.IsNullOrEmpty(cpf)) return null;
            return _context.Pessoas
                .AsNoTracking()
                .Include(p => p.IdDisciplinas)
                .FirstOrDefault(p => p.Cpf == cpf);
        }

        /// <summary>
        /// Busca uma pessoa pelo e‑mail.
        /// </summary>
        /// <param name="email">E‑mail da pessoa</param>
        /// <returns>Objeto pessoa ou null</returns>
        public Pessoa? GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return null;
            return _context.Pessoas
                .AsNoTracking()
                .FirstOrDefault(p => p.Email == email);
        }

        /// <summary>
        /// Retorna todas as pessoas ordenadas pelo nome.
        /// </summary>
        /// <returns>Lista de pessoas</returns>
        public IEnumerable<Pessoa> GetAll()
        {
            return _context.Pessoas
                .AsNoTracking()
                .Include(p => p.IdCidadeNavigation)
                .OrderBy(p => p.Nome)
                .ToList();
        }

        /// <summary>
        /// Busca pessoas cujo nome contenha o termo informado.
        /// </summary>
        /// <param name="nome">Termo de busca</param>
        /// <returns>Lista de pessoas</returns>
        public IEnumerable<Pessoa> GetByNome(string nome)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.Nome.Contains(nome))
                .OrderBy(p => p.Nome)
                .ToList();
        }

        /// <summary>
        /// Retorna todos os professores.
        /// </summary>
        /// <returns>Lista de professores</returns>
        public IEnumerable<Pessoa> GetAllProfessores()
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.TipoPessoa == TipoProfessor)
                .OrderBy(p => p.Nome)
                .ToList();
        }

        /// <summary>
        /// Retorna todos os responsáveis.
        /// </summary>
        /// <returns>Lista de responsáveis</returns>
        public IEnumerable<Pessoa> GetAllResponsaveis()
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.TipoPessoa == TipoResponsavel)
                .OrderBy(p => p.Nome)
                .ToList();
        }

        /// <summary>
        /// Verifica se um responsável ainda precisa cadastrar alunos dentro do seu limite.
        /// </summary>
        /// <param name="cpfLogado">CPF do responsável</param>
        /// <returns>True se ainda há dependentes a cadastrar, False caso contrário</returns>
        public bool ResponsavelPrecisaCadastrarAlunos(string? cpfLogado)
        {
            var resp = GetByCpf(cpfLogado);
            if (resp?.TipoPessoa != TipoResponsavel) return false;

            var limite = resp.QuantidadeDeDependentes ?? 0;
            if (limite <= 0) return false;

            var cadastrados = _context.Pessoas.Count(p => p.TipoPessoa == TipoAluno && p.ResponsavelId == resp.Id);
            return cadastrados < limite;
        }

        /// <summary>
        /// Atualiza o e‑mail de uma pessoa.
        /// </summary>
        /// <param name="pessoaId">ID da pessoa</param>
        /// <param name="novoEmail">Novo e‑mail</param>
        public void AtualizarEmailPessoa(int pessoaId, string novoEmail)
        {
            var pessoa = _context.Pessoas.Find(pessoaId);
            if (pessoa != null)
            {
                pessoa.Email = novoEmail;
                _context.SaveChanges();
            }
        }

        // ========== MÉTODOS PRIVADOS ==========

        /// <summary>
        /// Valida se o CPF já está cadastrado.
        /// </summary>
        /// <param name="cpf">CPF a ser verificado</param>
        /// <exception cref="ServiceException">Lançada se o CPF já existir</exception>
        private void ValidarCpfUnico(string cpf)
        {
            if (_context.Pessoas.AsNoTracking().Any(p => p.Cpf == cpf))
                throw new ServiceException("Já existe um usuário cadastrado com este CPF.");
        }

        /// <summary>
        /// Valida se o e‑mail já está cadastrado (exceto para alunos).
        /// </summary>
        /// <param name="email">E‑mail a ser verificado</param>
        /// <param name="tipoPessoa">Tipo de pessoa (alunos podem ter e‑mail duplicado)</param>
        /// <exception cref="ServiceException">Lançada se o e‑mail já existir para não‑alunos</exception>
        private void ValidarEmailUnico(string email, string tipoPessoa)
        {
            if (tipoPessoa != TipoAluno && _context.Pessoas.AsNoTracking().Any(p => p.Email == email))
                throw new ServiceException("Este e-mail já está sendo utilizado por outro usuário.");
        }

        /// <summary>
        /// Dispara as validações específicas conforme o tipo de pessoa.
        /// </summary>
        /// <param name="pessoa">Objeto pessoa</param>
        private void ValidarDadosEspecificos(Pessoa pessoa)
        {
            switch (pessoa.TipoPessoa)
            {
                case TipoProfessor:
                    ValidarProfessor(pessoa);
                    break;
                case TipoAluno:
                    ValidarAlunoSemLimite(pessoa);
                    break;
                case TipoResponsavel:
                    ValidarResponsavel(pessoa);
                    break;
                default:
                    throw new ServiceException("Tipo de pessoa inválido.");
            }
        }

        /// <summary>
        /// Valida os dados obrigatórios de um professor.
        /// </summary>
        /// <param name="pessoa">Objeto pessoa do tipo professor</param>
        private void ValidarProfessor(Pessoa pessoa)
        {
            if (string.IsNullOrEmpty(pessoa.DescricaoProfessor))
                throw new ServiceException("O perfil de professor exige uma descrição profissional.");

            if (pessoa.IdDisciplinas != null && pessoa.IdDisciplinas.Any())
            {
                foreach (var disciplina in pessoa.IdDisciplinas)
                {
                    _context.Attach(disciplina);
                }
            }
        }

        /// <summary>
        /// Valida os dados de um aluno (sem verificar limite de dependentes – feito na transação).
        /// </summary>
        /// <param name="pessoa">Objeto pessoa do tipo aluno</param>
        private void ValidarAlunoSemLimite(Pessoa pessoa)
        {
            if (pessoa.ResponsavelId == null || pessoa.ResponsavelId <= 0)
                throw new ServiceException("Um aluno não pode ser cadastrado sem um responsável vinculado.");

            var responsavel = _context.Pessoas
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == pessoa.ResponsavelId && p.TipoPessoa == TipoResponsavel)
                ?? throw new ServiceException("O responsável informado é inválido ou não existe.");
        }

        /// <summary>
        /// Valida os dados de um responsável.
        /// </summary>
        /// <param name="pessoa">Objeto pessoa do tipo responsável</param>
        private void ValidarResponsavel(Pessoa pessoa)
        {
            if (pessoa.QuantidadeDeDependentes < 0)
                throw new ServiceException("A quantidade de dependentes não pode ser negativa.");
        }

        /// <summary>
        /// Atualiza os dados comuns (nome, endereço, etc.) de uma pessoa.
        /// </summary>
        /// <param name="original">Entidade original</param>
        /// <param name="atualizada">Entidade com novos dados</param>
        private void AtualizarDadosComuns(Pessoa original, Pessoa atualizada)
        {
            original.Nome = atualizada.Nome;
            original.Sobrenome = atualizada.Sobrenome;
            original.Email = atualizada.Email;
            original.Telefone = atualizada.Telefone;
            original.Genero = atualizada.Genero;
            original.DataNascimento = atualizada.DataNascimento;
            original.Cep = atualizada.Cep;
            original.Rua = atualizada.Rua;
            original.Numero = atualizada.Numero;
            original.Complemento = atualizada.Complemento;
            original.Bairro = atualizada.Bairro;
            original.Cidade = atualizada.Cidade;
            original.Estado = atualizada.Estado;
        }

        /// <summary>
        /// Atualiza os arquivos (diploma, foto) se novos forem fornecidos.
        /// </summary>
        /// <param name="original">Entidade original</param>
        /// <param name="atualizada">Entidade com novos arquivos</param>
        private void AtualizarArquivos(Pessoa original, Pessoa atualizada)
        {
            if (atualizada.Diploma?.Length > 0)
                original.Diploma = atualizada.Diploma;
            if (atualizada.FotoPerfil?.Length > 0)
                original.FotoPerfil = atualizada.FotoPerfil;
            if (atualizada.FotoDocumento?.Length > 0)
                original.FotoDocumento = atualizada.FotoDocumento;
        }

        /// <summary>
        /// Atualiza os dados específicos conforme o tipo de pessoa.
        /// </summary>
        /// <param name="original">Entidade original</param>
        /// <param name="atualizada">Entidade com novos dados</param>
        private void AtualizarDadosEspecificos(Pessoa original, Pessoa atualizada)
        {
            if (original.TipoPessoa == TipoProfessor)
            {
                original.DescricaoProfessor = atualizada.DescricaoProfessor;
                original.Libras = atualizada.Libras;
                original.IdCidade = atualizada.IdCidade;
                AtualizarDisciplinas(original, atualizada);
            }
            else if (original.TipoPessoa == TipoAluno)
            {
                original.AlunoDeMenor = atualizada.AlunoDeMenor;
                original.Atipico = atualizada.Atipico;
            }
            else if (original.TipoPessoa == TipoResponsavel)
            {
                original.QuantidadeDeDependentes = atualizada.QuantidadeDeDependentes;
            }
        }

        /// <summary>
        /// Atualiza a lista de disciplinas de um professor.
        /// </summary>
        /// <param name="original">Entidade original</param>
        /// <param name="atualizada">Entidade com nova lista de disciplinas</param>
        private void AtualizarDisciplinas(Pessoa original, Pessoa atualizada)
        {
            if (atualizada.IdDisciplinas == null) return;

            original.IdDisciplinas.Clear();
            foreach (var disciplina in atualizada.IdDisciplinas)
            {
                var disciplinaLocal = _context.Disciplinas.Local.FirstOrDefault(d => d.Id == disciplina.Id);
                if (disciplinaLocal != null)
                {
                    original.IdDisciplinas.Add(disciplinaLocal);
                }
                else
                {
                    _context.Attach(disciplina);
                    original.IdDisciplinas.Add(disciplina);
                }
            }
        }

        /// <summary>
        /// Verifica se o usuário logado tem permissão para acessar/editar/excluir uma pessoa.
        /// </summary>
        /// <param name="id">ID da pessoa alvo</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <param name="entity">Entidade encontrada (out)</param>
        /// <returns>True se tem permissão, False caso contrário</returns>
        private bool TemPermissao(int id, string? cpfLogado, bool isAdmin, out Pessoa? entity)
        {
            entity = Get(id);
            if (entity == null) return false;

            if (isAdmin) return true;

            var usuarioRaiz = GetByCpf(cpfLogado);
            if (usuarioRaiz == null) return false;

            return entity.Id == usuarioRaiz.Id ||
                   (usuarioRaiz.TipoPessoa == TipoResponsavel && entity.ResponsavelId == usuarioRaiz.Id);
        }

        /// <summary>
        /// Verifica se o usuário logado pode visualizar uma pessoa.
        /// </summary>
        /// <param name="entity">Entidade alvo</param>
        /// <param name="cpfLogado">CPF do usuário logado</param>
        /// <param name="isAdmin">Indica se o usuário logado é administrador</param>
        /// <returns>True se pode visualizar, False caso contrário</returns>
        private bool PodeVer(Pessoa? entity, string? cpfLogado, bool isAdmin)
        {
            if (entity == null) return false;
            if (isAdmin) return true;

            var usuarioRaiz = GetByCpf(cpfLogado);
            if (usuarioRaiz == null) return false;

            return entity.Id == usuarioRaiz.Id ||
                   (usuarioRaiz.TipoPessoa == TipoResponsavel && entity.ResponsavelId == usuarioRaiz.Id);
        }
    }
}