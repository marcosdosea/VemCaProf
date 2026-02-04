using Core;          
using Core.Service; 
using Microsoft.EntityFrameworkCore;


namespace Service
{
    public class PessoaService : IPessoaService
    {
        private readonly VemCaProfContext _context;

        public PessoaService(VemCaProfContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Cria uma nova Pessoa.
        /// O Controller já deve ter preenchido o TipoPessoa ("P", "A" ou "R").
        /// </summary>
        /// <param name="pessoa"> Criação de uma nova pessoa dependente do tipo</param>
        public int Create(Pessoa pessoa)
        {
            // Regra Geral: CPF Único (Vale para todos)
            if (_context.Pessoas.AsNoTracking().Any(p => p.Cpf == pessoa.Cpf))
            {
                throw new ServiceException("Já existe um usuário cadastrado com este CPF.");
            }
            // Unicidade do E-mail
            if (_context.Pessoas.AsNoTracking().Any(p => p.Email == pessoa.Email))
            {
                throw new ServiceException("Este e-mail já está sendo utilizado por outro usuário.");
            }

            // 2. Validações por Tipo
            switch (pessoa.TipoPessoa)
            {
                case "P": // PROFESSOR
                    if (string.IsNullOrEmpty(pessoa.DescricaoProfessor))
                        throw new ServiceException("O perfil de professor exige uma descrição profissional.");
            
                    // Tratamento das Disciplinas (N:M)
                    if (pessoa.IdDisciplinas != null && pessoa.IdDisciplinas.Any())
                    {
                        foreach (var disciplina in pessoa.IdDisciplinas)
                        {
                            _context.Attach(disciplina);
                        }
                    }
                    break;

                case "A": // ALUNO
                    // Validação de Vínculo: Aluno precisa de um responsavel
                    if (pessoa.ResponsavelId == null || pessoa.ResponsavelId <= 0)
                    {
                        throw new ServiceException("Um aluno não pode ser cadastrado sem um responsável vinculado.");
                    }

                    // Busca o Responsavel para validar a cota de dependentes
                    var responsavel = _context.Pessoas
                        .AsNoTracking()
                        .FirstOrDefault(p => p.Id == pessoa.ResponsavelId && p.TipoPessoa == "R");

                    if (responsavel == null)
                    {
                        throw new ServiceException("O responsável informado é inválido ou não existe.");
                    }

                    // Verificação da Cota de Dependentes
                    int dependentesAtuais = _context.Pessoas
                        .Count(p => p.ResponsavelId == pessoa.ResponsavelId && p.TipoPessoa == "A");

                    if (dependentesAtuais >= responsavel.QuantidadeDeDependentes)
                    {
                        throw new ServiceException($"Limite de dependentes atingido ({responsavel.QuantidadeDeDependentes}). Aumente o limite no perfil do responsável.");
                    }
                    break;

                case "R": // RESPONSÁVEL
                    if (pessoa.QuantidadeDeDependentes < 0)
                        throw new ServiceException("A quantidade de dependentes não pode ser negativa.");
                    break;

                default:
                    throw new ServiceException("Tipo de pessoa inválido.");
            }

            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();
            return pessoa.Id;
        }

        /// <summary>
        /// Edita uma Pessoa.
        /// Implementação manual para garantir integridade de Arquivos e Relacionamentos.
        /// </summary>
        /// <param name="pessoaAtualizada"> Os novos atributos de Pessoa</param>
        public void Edit(Pessoa pessoaAtualizada)
        {
            var pessoaOriginal = _context.Pessoas
                .Include(p => p.IdDisciplinas) 
                .FirstOrDefault(p => p.Id == pessoaAtualizada.Id);

            if (pessoaOriginal == null)
                throw new ServiceException("Pessoa não encontrada.");

            // Atualiza Dados Comuns 
            pessoaOriginal.Nome = pessoaAtualizada.Nome;
            pessoaOriginal.Sobrenome = pessoaAtualizada.Sobrenome;
            pessoaOriginal.Email = pessoaAtualizada.Email;
            pessoaOriginal.Telefone = pessoaAtualizada.Telefone;
            pessoaOriginal.Genero = pessoaAtualizada.Genero;
            pessoaOriginal.DataNascimento = pessoaAtualizada.DataNascimento;

            // Endereço
            pessoaOriginal.Cep = pessoaAtualizada.Cep;
            pessoaOriginal.Rua = pessoaAtualizada.Rua;
            pessoaOriginal.Numero = pessoaAtualizada.Numero;
            pessoaOriginal.Complemento = pessoaAtualizada.Complemento;
            pessoaOriginal.Bairro = pessoaAtualizada.Bairro;
            pessoaOriginal.Cidade = pessoaAtualizada.Cidade;
            pessoaOriginal.Estado = pessoaAtualizada.Estado;

            // Proteção de Arquivos (Uploads)
            
            if (pessoaAtualizada.Diploma != null && pessoaAtualizada.Diploma.Length > 0)
            {
                pessoaOriginal.Diploma = pessoaAtualizada.Diploma;
            }

            if (pessoaAtualizada.FotoPerfil != null && pessoaAtualizada.FotoPerfil.Length > 0)
            {
                pessoaOriginal.FotoPerfil = pessoaAtualizada.FotoPerfil;
            }
            
            if (pessoaAtualizada.FotoDocumento != null && pessoaAtualizada.FotoDocumento.Length > 0)
            {
                pessoaOriginal.FotoDocumento = pessoaAtualizada.FotoDocumento;
            }

            // 4. Atualização Específica baseada no Tipo 
            if (pessoaOriginal.TipoPessoa == "P") // Professor
            {
                pessoaOriginal.DescricaoProfessor = pessoaAtualizada.DescricaoProfessor;
                pessoaOriginal.Libras = pessoaAtualizada.Libras;
                pessoaOriginal.IdCidade = pessoaAtualizada.IdCidade;

                if (pessoaAtualizada.IdDisciplinas != null)
                {
                    pessoaOriginal.IdDisciplinas.Clear();
                    
                    foreach (var disciplina in pessoaAtualizada.IdDisciplinas)
                    {
                        _context.Attach(disciplina); 
                        pessoaOriginal.IdDisciplinas.Add(disciplina);
                    }
                }
            }
            else if (pessoaOriginal.TipoPessoa == "A") // Aluno
            {
                pessoaOriginal.AlunoDeMenor = pessoaAtualizada.AlunoDeMenor;
                pessoaOriginal.Atipico = pessoaAtualizada.Atipico;
                pessoaOriginal.ResponsavelId = pessoaAtualizada.ResponsavelId;
            }
            else if (pessoaOriginal.TipoPessoa == "R") // Responsável
            {
                pessoaOriginal.QuantidadeDeDependentes = pessoaAtualizada.QuantidadeDeDependentes;
            }

            // 5. Salva as alterações
            _context.Update(pessoaOriginal);
            _context.SaveChanges();
        }
        /// <summary>
        /// Remove uma Pessoa.
        /// </summary>
        /// <param name="id">id da Pessoa</param>
        public void Delete(int id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if (pessoa != null)
            {
                _context.Pessoas.Remove(pessoa);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Busca uma Pessoa pelo id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ServiceException"></exception>
        public Pessoa Get(int id)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Include(p => p.IdDisciplinas) 
                .FirstOrDefault(p => p.Id == id) 
                ?? throw new ServiceException("Pessoa não encontrada.");
        }

        /// <summary>
        /// Busca uma pessoa pelo CPF 
        /// </summary>
        /// <param name="cpf">Busca a pessoa pelo CPF</param>
        public Pessoa? GetByCpf(string cpf)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Include(p => p.IdDisciplinas)
                .FirstOrDefault(p => p.Cpf == cpf);
        }
        
        /// <summary>
        /// Retorna todas as Pessoas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Pessoa> GetAll()
        {
            return _context.Pessoas
                .AsNoTracking()
                .Include(p => p.IdCidadeNavigation)
                .OrderBy(p => p.Nome)
                .ToList(); 
        }
    
        /// <summary>
        /// Retorna Pessoas pelo nome.
        /// </summary>
        /// <param name="nome"> Retorna a pessoa de acordo com o nome</param>
        /// <returns></returns>
        public IEnumerable<Pessoa> GetByNome(string nome)
        {
            return _context.Pessoas
                .AsNoTracking()
                .Where(p => p.Nome.Contains(nome))
                .OrderBy(p => p.Nome)
                .ToList();
        }
    }
}