using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceTests
{
    [TestClass]
    public class PessoaServiceTests
    {
        private VemCaProfContext _context = null!;
        private IPessoaService _pessoaService = null!;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<VemCaProfContext>();
            builder.UseInMemoryDatabase($"VCP_{Guid.NewGuid()}");
            builder.ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var options = builder.Options;

            _context = new VemCaProfContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            SeedData();

            _pessoaService = new PessoaService(_context);
        }

        private void SeedData()
        {
            var disciplinas = new List<Disciplina>
            {
                new Disciplina { Id = 1u, Nome = "Matemática", Descricao = "Matemática básica", Nivel = "F2" },
                new Disciplina { Id = 2u, Nome = "Português", Descricao = "Língua Portuguesa", Nivel = "F2" },
                new Disciplina { Id = 3u, Nome = "História", Descricao = "História Geral", Nivel = "M1" }
            };
            _context.Disciplinas.AddRange(disciplinas);

            var pessoas = new List<Pessoa>
            {
                // ID 1: Professor
                new Pessoa
                {
                    Id = 1,
                    Nome = "Professor",
                    Sobrenome = "Girafales",
                    Cpf = "11111111111",
                    Email = "prof.girafales@escola.com",
                    Telefone = "11911111111",
                    Genero = "M",
                    DataNascimento = new DateTime(1970, 5, 10),
                    Cep = "01001000",
                    Rua = "Rua das Flores",
                    Numero = "100",
                    Complemento = "Apto 101",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    TipoPessoa = "P",
                    DescricaoProfessor = "Professor de Matemática e Física",
                    Libras = true,
                    IdCidade = 1,
                    IdDisciplinas = new List<Disciplina> { disciplinas[0], disciplinas[1] }
                },

                // ID 2: Aluno
                new Pessoa
                {
                    Id = 2,
                    Nome = "Chaves",
                    Sobrenome = "do Oito",
                    Cpf = "22222222222",
                    Email = "chaves@vila.com",
                    Telefone = "11922222222",
                    Genero = "M",
                    DataNascimento = new DateTime(2015, 7, 20),
                    Cep = "02002000",
                    Rua = "Rua da Vila",
                    Numero = "8",
                    Complemento = "Barril",
                    Bairro = "Vila",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    TipoPessoa = "A",
                    AlunoDeMenor = true,
                    Atipico = false,
                    ResponsavelId = 3
                },

                // ID 3: Responsável
                new Pessoa
                {
                    Id = 3,
                    Nome = "Seu",
                    Sobrenome = "Madruga",
                    Cpf = "33333333333",
                    Email = "madruga@vila.com",
                    Telefone = "11933333333",
                    Genero = "M",
                    DataNascimento = new DateTime(1980, 2, 15),
                    Cep = "03003000",
                    Rua = "Rua da Reforma",
                    Numero = "42",
                    Complemento = "Casa",
                    Bairro = "Vila",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    TipoPessoa = "R",
                    QuantidadeDeDependentes = 2
                }
            };

            _context.Pessoas.AddRange(pessoas);
            _context.SaveChanges();
            
            var responsavel = _context.Pessoas.Find(3);
            responsavel.InverseResponsavel = new List<Pessoa> { pessoas[1] };
            _context.SaveChanges();
        }

        // ==================== TESTES DE PROFESSOR ====================

        [TestMethod]
        public void CreateProfessor_ComDadosValidos_DevePersistir()
        {
            // Arrange
            var disciplina = _context.Disciplinas.Find(3u);
            var professor = new Pessoa
            {
                Nome = "Novo",
                Sobrenome = "Professor",
                Cpf = "44444444444",
                Email = "novo.prof@escola.com",
                Telefone = "11944444444",
                Genero = "M",
                DataNascimento = new DateTime(1985, 3, 10),
                Cep = "04004000",
                Rua = "Rua Nova",
                Numero = "200",
                Complemento = "Sala 3",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                TipoPessoa = "P",
                DescricaoProfessor = "Professor de História",
                Libras = false,
                IdCidade = 2,
                IdDisciplinas = new List<Disciplina> { disciplina }
            };

            // Act
            var id = _pessoaService.Create(professor);
            var criado = _pessoaService.Get(id);

            // Assert
            Assert.IsNotNull(criado);
            Assert.AreEqual("Novo", criado.Nome);
            Assert.AreEqual("Professor de História", criado.DescricaoProfessor);
            Assert.AreEqual(1, criado.IdDisciplinas.Count);
        }

        [TestMethod]
        public void EditProfessor_AlterarDados_DeveAtualizar()
        {
            // Arrange
            var professor = _pessoaService.Get(1);
            professor.Nome = "Professor Alterado";
            professor.DescricaoProfessor = "Agora ensina Física";
            professor.IdCidade = 99;
            professor.Libras = false;

            // Act
            _pessoaService.Edit(professor);
            var editado = _pessoaService.Get(1);

            // Assert
            Assert.AreEqual("Professor Alterado", editado.Nome);
            Assert.AreEqual("Agora ensina Física", editado.DescricaoProfessor);
            Assert.AreEqual(99, editado.IdCidade);
            Assert.IsFalse(editado.Libras);
        }

        [TestMethod]
        public void GetProfessor_PorId_DeveRetornar()
        {
            var professor = _pessoaService.Get(1);
            Assert.IsNotNull(professor);
            Assert.AreEqual("Professor", professor.Nome);
            Assert.AreEqual("Girafales", professor.Sobrenome);
            Assert.AreEqual("11111111111", professor.Cpf);
        }

        [TestMethod]
        public void GetAllProfessores_DeveRetornarApenasProfessores()
        {
            var professores = _pessoaService.GetAllProfessores();
            Assert.AreEqual(1, professores.Count());
            Assert.IsTrue(professores.All(p => p.TipoPessoa == "P"));
        }

        // ==================== TESTES DE ALUNO ====================

        [TestMethod]
        public void CreateAluno_ComResponsavelValido_DevePersistir()
        {
            // Arrange
            var aluno = new Pessoa
            {
                Nome = "Kiko",
                Sobrenome = "Tesouro",
                Cpf = "55555555555",
                Email = "kiko@vila.com",
                Telefone = "11955555555",
                Genero = "M",
                DataNascimento = new DateTime(2016, 8, 10),
                Cep = "05005000",
                Rua = "Rua do Tesouro",
                Numero = "7",
                Complemento = "Baú",
                Bairro = "Vila",
                Cidade = "São Paulo",
                Estado = "SP",
                TipoPessoa = "A",
                AlunoDeMenor = true,
                Atipico = false,
                ResponsavelId = 3
            };

            // Act
            var id = _pessoaService.Create(aluno);
            var criado = _pessoaService.Get(id);

            // Assert
            Assert.IsNotNull(criado);
            Assert.AreEqual("Kiko", criado.Nome);
            Assert.AreEqual(3, criado.ResponsavelId);
        }

        [TestMethod]
        public void EditAluno_AlterarDados_DeveAtualizar()
        {
            // Arrange
            var aluno = _pessoaService.Get(2);
            aluno.Nome = "Chaves do Oito";
            aluno.AlunoDeMenor = false;
            aluno.Atipico = true;

            // Act
            _pessoaService.Edit(aluno);
            var editado = _pessoaService.Get(2);

            // Assert
            Assert.AreEqual("Chaves do Oito", editado.Nome);
            Assert.IsFalse(editado.AlunoDeMenor);
            Assert.IsTrue(editado.Atipico);
        }

        // ==================== TESTES DE RESPONSÁVEL ====================

        [TestMethod]
        public void CreateResponsavel_ComDadosValidos_DevePersistir()
        {
            // Arrange
            var responsavel = new Pessoa
            {
                Nome = "Dona",
                Sobrenome = "Florinda",
                Cpf = "66666666666",
                Email = "florinda@vila.com",
                Telefone = "11966666666",
                Genero = "F",
                DataNascimento = new DateTime(1975, 6, 5),
                Cep = "06006000",
                Rua = "Rua da Bruxa",
                Numero = "13",
                Complemento = "Casa assombrada",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP",
                TipoPessoa = "R",
                QuantidadeDeDependentes = 3
            };

            // Act
            var id = _pessoaService.Create(responsavel);
            var criado = _pessoaService.Get(id);

            // Assert
            Assert.IsNotNull(criado);
            Assert.AreEqual("Dona", criado.Nome);
            Assert.AreEqual(3, criado.QuantidadeDeDependentes);
        }

        [TestMethod]
        public void EditResponsavel_AlterarQuantidadeDependentes_DeveAtualizar()
        {
            // Arrange
            var responsavel = _pessoaService.Get(3);
            responsavel.QuantidadeDeDependentes = 5;

            // Act
            _pessoaService.Edit(responsavel);
            var editado = _pessoaService.Get(3);

            // Assert
            Assert.AreEqual(5, editado.QuantidadeDeDependentes);
        }

        [TestMethod]
        public void GetAllResponsaveis_DeveRetornarApenasResponsaveis()
        {
            var responsaveis = _pessoaService.GetAllResponsaveis();
            Assert.AreEqual(1, responsaveis.Count());
            Assert.IsTrue(responsaveis.All(r => r.TipoPessoa == "R"));
        }

        // ==================== TESTES DE VALIDAÇÃO (EXCEÇÕES) ====================

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void CreateProfessor_CpfDuplicado_DeveLancarExcecao()
        {
            var professor = new Pessoa
            {
                Nome = "Duplicado",
                Sobrenome = "Teste",
                Cpf = "11111111111", // CPF já existente
                Email = "unico@email.com",
                Telefone = "11999999999",
                Genero = "M",
                DataNascimento = DateTime.Now.AddYears(-30),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Casa",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "P",
                DescricaoProfessor = "Descrição"
            };
            _pessoaService.Create(professor);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void CreateProfessor_EmailDuplicado_DeveLancarExcecao()
        {
            var professor = new Pessoa
            {
                Nome = "Duplicado",
                Sobrenome = "Teste",
                Cpf = "77777777777", // CPF diferente
                Email = "prof.girafales@escola.com", // e-mail já usado
                Telefone = "11999999999",
                Genero = "M",
                DataNascimento = DateTime.Now.AddYears(-30),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Casa",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "P",
                DescricaoProfessor = "Descrição"
            };
            _pessoaService.Create(professor);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void CreateAluno_ResponsavelInvalido_DeveLancarExcecao()
        {
            var aluno = new Pessoa
            {
                Nome = "Aluno",
                Sobrenome = "SemResponsavel",
                Cpf = "88888888888",
                Email = "sem.resp@email.com",
                Telefone = "11988888888",
                Genero = "M",
                DataNascimento = new DateTime(2010, 1, 1),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Apto",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "A",
                ResponsavelId = 999 // ID inexistente
            };
            _pessoaService.Create(aluno);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void CreateAluno_ExcedeLimiteDependentes_DeveLancarExcecao()
        {
            // Responsável 3 tem limite = 2, já tem 1 dependente (Chaves)
            // Criar primeiro aluno dentro do limite
            var aluno1 = new Pessoa
            {
                Nome = "Primeiro",
                Sobrenome = "Dependente",
                Cpf = "99999999999",
                Email = "primeiro@vila.com",
                Telefone = "11999999999",
                Genero = "M",
                DataNascimento = new DateTime(2012, 3, 3),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Casa",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "A",
                ResponsavelId = 3
            };
            var id1 = _pessoaService.Create(aluno1);
            Assert.IsTrue(id1 > 0);

            // Verificar que agora existem 2 dependentes
            var count = _context.Pessoas.Count(p => p.ResponsavelId == 3 && p.TipoPessoa == "A");
            Assert.AreEqual(2, count);

            // Tentar criar o terceiro (deve exceder)
            var aluno2 = new Pessoa
            {
                Nome = "Segundo",
                Sobrenome = "Excedente",
                Cpf = "10101010101",
                Email = "segundo@vila.com",
                Telefone = "11910101010",
                Genero = "M",
                DataNascimento = new DateTime(2014, 4, 4),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Apto",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "A",
                ResponsavelId = 3
            };
            _pessoaService.Create(aluno2); // deve lançar ServiceException
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void DeleteResponsavel_ComDependentes_DeveLancarExcecao()
        {
            _pessoaService.Delete(3); // Responsável com dependente (Chaves)
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void CreateProfessor_SemDescricao_DeveLancarExcecao()
        {
            var professor = new Pessoa
            {
                Nome = "Professor",
                Sobrenome = "SemDescricao",
                Cpf = "12121212121",
                Email = "semdesc@email.com",
                Telefone = "11912121212",
                Genero = "M",
                DataNascimento = DateTime.Now.AddYears(-40),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Casa",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "P",
                DescricaoProfessor = "" // descrição vazia
            };
            _pessoaService.Create(professor);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void CreateResponsavel_QuantidadeDependentesNegativa_DeveLancarExcecao()
        {
            var responsavel = new Pessoa
            {
                Nome = "Responsável",
                Sobrenome = "Negativo",
                Cpf = "13131313131",
                Email = "negativo@email.com",
                Telefone = "11913131313",
                Genero = "F",
                DataNascimento = DateTime.Now.AddYears(-35),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Casa",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "R",
                QuantidadeDeDependentes = -1
            };
            _pessoaService.Create(responsavel);
        }

        // ==================== TESTES DOS MÉTODOS "SEGURO" ====================

        [TestMethod]
        public void CreateSeguro_Professor_ComPermissao_DeveCriar()
        {
            var professor = new Pessoa
            {
                Nome = "Seguro",
                Sobrenome = "Professor",
                Cpf = "14141414141",
                Email = "seguro.prof@email.com",
                Telefone = "11914141414",
                Genero = "M",
                DataNascimento = DateTime.Now.AddYears(-30),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Sala 10",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "P",
                DescricaoProfessor = "Professor seguro"
            };
            var cpfLogado = "99999999999"; // qualquer CPF
            _pessoaService.CreateSeguro(professor, cpfLogado, isAdmin: false, isProfessor: true, isResponsavel: false);
            var criado = _pessoaService.GetByCpf(professor.Cpf);
            Assert.IsNotNull(criado);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceException))]
        public void CreateSeguro_Professor_SemPermissao_DeveLancarExcecao()
        {
            var professor = new Pessoa
            {
                Nome = "Seguro",
                Sobrenome = "Professor",
                Cpf = "15151515151",
                Email = "seguro.prof2@email.com",
                Telefone = "11915151515",
                Genero = "M",
                DataNascimento = DateTime.Now.AddYears(-30),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Sala 20",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "P",
                DescricaoProfessor = "Professor sem permissão"
            };
            _pessoaService.CreateSeguro(professor, "logado", isAdmin: false, isProfessor: false, isResponsavel: false);
        }

        [TestMethod]
        public void CreateSeguro_Aluno_ComoResponsavel_DeveVincularResponsavel()
        {
            var aluno = new Pessoa
            {
                Nome = "Aluno",
                Sobrenome = "Seguro",
                Cpf = "16161616161",
                Email = "aluno.seguro@email.com",
                Telefone = "11916161616",
                Genero = "M",
                DataNascimento = new DateTime(2010, 1, 1),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Apto",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "A",
                AlunoDeMenor = true
                // ResponsavelId não será informado, deve ser preenchido pelo CreateSeguro
            };
            var cpfLogado = "33333333333"; // CPF do responsável Seu Madruga
            _pessoaService.CreateSeguro(aluno, cpfLogado, isAdmin: false, isProfessor: false, isResponsavel: true);
            var criado = _pessoaService.GetByCpf(aluno.Cpf);
            Assert.IsNotNull(criado);
            Assert.AreEqual(3, criado.ResponsavelId);
        }

        [TestMethod]
        public void EditSeguro_ProprioUsuario_DevePermitir()
        {
            var professor = _pessoaService.Get(1);
            professor.Nome = "Editado por si mesmo";
            var cpfLogado = "11111111111"; // CPF do próprio professor
            var resultado = _pessoaService.EditSeguro(professor, cpfLogado, isAdmin: false);
            Assert.IsTrue(resultado);
            var editado = _pessoaService.Get(1);
            Assert.AreEqual("Editado por si mesmo", editado.Nome);
        }

        [TestMethod]
        public void EditSeguro_Admin_DevePermitir()
        {
            var professor = _pessoaService.Get(1);
            professor.Nome = "Editado por admin";
            var cpfLogado = "99999999999"; // qualquer CPF
            var resultado = _pessoaService.EditSeguro(professor, cpfLogado, isAdmin: true);
            Assert.IsTrue(resultado);
            var editado = _pessoaService.Get(1);
            Assert.AreEqual("Editado por admin", editado.Nome);
        }

        [TestMethod]
        public void EditSeguro_OutroUsuario_SemPermissao_DeveRetornarFalse()
        {
            var professor = _pessoaService.Get(1);
            professor.Nome = "Tentativa inválida";
            var cpfLogado = "22222222222"; // CPF do aluno Chaves (não é admin nem o dono)
            var resultado = _pessoaService.EditSeguro(professor, cpfLogado, isAdmin: false);
            Assert.IsFalse(resultado);
            var naoAlterado = _pessoaService.Get(1);
            Assert.AreNotEqual("Tentativa inválida", naoAlterado.Nome);
        }

        [TestMethod]
        public void DeleteSeguro_ProprioUsuario_SemDependentes_DeveExcluir()
        {
            // Criar um novo professor para excluir (sem dependentes)
            var professor = new Pessoa
            {
                Nome = "Excluir",
                Sobrenome = "Teste",
                Cpf = "17171717171",
                Email = "excluir@email.com",
                Telefone = "11917171717",
                Genero = "M",
                DataNascimento = DateTime.Now.AddYears(-25),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Sala 30",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "P",
                DescricaoProfessor = "Será excluído"
            };
            var id = _pessoaService.Create(professor);
            var resultado = _pessoaService.DeleteSeguro(id, professor.Cpf, isAdmin: false);
            Assert.IsTrue(resultado);
            Assert.IsNull(_pessoaService.Get(id));
        }

        [TestMethod]
        public void GetParaDetails_ProprioUsuario_DeveRetornar()
        {
            var pessoa = _pessoaService.GetParaDetails(1, "11111111111", isAdmin: false);
            Assert.IsNotNull(pessoa);
        }

        [TestMethod]
        public void GetParaDetails_OutroUsuario_SemPermissao_DeveRetornarNull()
        {
            var pessoa = _pessoaService.GetParaDetails(1, "22222222222", isAdmin: false);
            Assert.IsNull(pessoa);
        }

        [TestMethod]
        public void GetParaDetails_Admin_DeveRetornarQualquer()
        {
            var pessoa = _pessoaService.GetParaDetails(1, "admin", isAdmin: true);
            Assert.IsNotNull(pessoa);
        }

        // ==================== TESTES DE LISTAGEM E INDEX ====================

        [TestMethod]
        public void GetListaParaIndex_Admin_DeveRetornarTodos()
        {
            var lista = _pessoaService.GetListaParaIndex(null, "admin", isAdmin: true);
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod]
        public void GetListaParaIndex_AdminComFiltro_DeveFiltrarPorTipo()
        {
            var professores = _pessoaService.GetListaParaIndex("P", "admin", isAdmin: true);
            Assert.AreEqual(1, professores.Count());
            Assert.AreEqual("P", professores.First().TipoPessoa);
        }

        [TestMethod]
        public void GetListaParaIndex_Responsavel_DeveVerProprioPerfilEDependentes()
        {
            var lista = _pessoaService.GetListaParaIndex(null, "33333333333", isAdmin: false);
            Assert.AreEqual(2, lista.Count()); // responsável (ID 3) + dependente (ID 2)
            Assert.IsTrue(lista.Any(p => p.Id == 3));
            Assert.IsTrue(lista.Any(p => p.Id == 2));
        }

        [TestMethod]
        public void GetListaParaIndex_Aluno_DeveVerApenasProprioPerfil()
        {
            var lista = _pessoaService.GetListaParaIndex(null, "22222222222", isAdmin: false);
            Assert.AreEqual(1, lista.Count());
            Assert.AreEqual(2, lista.First().Id);
        }

        // ==================== TESTES DE MÉTODOS AUXILIARES ====================

        [TestMethod]
        public void GetByCpf_DeveRetornarPessoa()
        {
            var pessoa = _pessoaService.GetByCpf("11111111111");
            Assert.IsNotNull(pessoa);
            Assert.AreEqual(1, pessoa.Id);
        }

        [TestMethod]
        public void GetByEmail_DeveRetornarPessoa()
        {
            var pessoa = _pessoaService.GetByEmail("prof.girafales@escola.com");
            Assert.IsNotNull(pessoa);
        }

        [TestMethod]
        public void GetByNome_DeveRetornarPessoasComNomeContendo()
        {
            var pessoas = _pessoaService.GetByNome("Chaves");
            Assert.AreEqual(1, pessoas.Count());
        }

        [TestMethod]
        public void GetAll_DeveRetornarTodas()
        {
            var todas = _pessoaService.GetAll();
            Assert.AreEqual(3, todas.Count());
        }

        [TestMethod]
        public void ResponsavelPrecisaCadastrarAlunos_QuandoAbaixoDoLimite_DeveRetornarTrue()
        {
            // Responsável ID 3 tem limite 2 e atualmente 1 dependente
            var precisa = _pessoaService.ResponsavelPrecisaCadastrarAlunos("33333333333");
            Assert.IsTrue(precisa);
        }

        [TestMethod]
        public void ResponsavelPrecisaCadastrarAlunos_QuandoAtingiuLimite_DeveRetornarFalse()
        {
            // Criar mais um dependente para atingir o limite
            var aluno = new Pessoa
            {
                Nome = "Segundo",
                Sobrenome = "Dependente",
                Cpf = "18181818181",
                Email = "segundo.dep@email.com",
                Telefone = "11918181818",
                Genero = "M",
                DataNascimento = new DateTime(2011, 1, 1),
                Cep = "00000000",
                Rua = "Rua",
                Numero = "1",
                Complemento = "Casa",
                Bairro = "B",
                Cidade = "C",
                Estado = "E",
                TipoPessoa = "A",
                ResponsavelId = 3
            };
            _pessoaService.Create(aluno);
            var precisa = _pessoaService.ResponsavelPrecisaCadastrarAlunos("33333333333");
            Assert.IsFalse(precisa);
        }

        [TestMethod]
        public void AtualizarEmailPessoa_DeveAlterar()
        {
            _pessoaService.AtualizarEmailPessoa(1, "novo.email@prof.com");
            var pessoa = _pessoaService.Get(1);
            Assert.AreEqual("novo.email@prof.com", pessoa.Email);
        }
    }
}