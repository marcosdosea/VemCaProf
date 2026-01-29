using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace ServiceTests
{
    [TestClass()]
    public class PessoaServiceTests
    {
        private VemCaProfContext context = null!;
        private IPessoaService pessoaService = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<VemCaProfContext>();
            builder.UseInMemoryDatabase("VCP");
            var options = builder.Options;

            context = new VemCaProfContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var pessoas = new List<Pessoa>
            {
                // ID 1: Professor (Identificado por ter DescricaoProfessor)
                new Pessoa 
                { 
                    Id = 1, 
                    Nome = "Professor Girafales", 
                    Sobrenome = "Madruga",
                    Cpf = "11111111111", 
                    Email = "prof@email.com",
                    Senha = "123",
                    DescricaoProfessor = "Matemática", // Campo discriminador
                    Libras = true,
                    IdCidade = 100,
                    Genero = "Masculino",
                    Complemento = "Casa X",
                    // Campos obrigatórios base
                    Telefone = "11999999999", Cep = "00000-000", Rua = "Rua A", Numero = "1", Bairro = "B", Cidade = "SP", Estado = "SP"
                },

                // ID 2: Aluno (Identificado por ter AlunoDeMenor)
                new Pessoa 
                { 
                    Id = 2, 
                    Nome = "Chaves", 
                    Sobrenome = "do Oito",
                    Cpf = "22222222222", 
                    Email = "chaves@email.com",
                    Senha = "123",
                    AlunoDeMenor = true, // Campo discriminador
                    Atipico = false,
                    Genero =  "Masculino",
                    Complemento = "Casa X",
                    // Campos obrigatórios base
                    Telefone = "11988888888", Cep = "00000-000", Rua = "Rua B", Numero = "2", Bairro = "B", Cidade = "SP", Estado = "SP"
                },

                // ID 3: Responsável (Identificado por ter QuantidadeDeDependentes)
                new Pessoa 
                { 
                    Id = 3, 
                    Nome = "Seu Madruga", 
                    Sobrenome = "Ramón",
                    Cpf = "33333333333", 
                    Email = "madruga@email.com",
                    Senha = "123",
                    QuantidadeDeDependentes = 1, // Campo discriminador
                    Genero = "Masculino",
                    Complemento = "Casa X",
                    // Campos obrigatórios base
                    Telefone = "11977777777", Cep = "00000-000", Rua = "Rua C", Numero = "3", Bairro = "B", Cidade = "SP", Estado = "SP"
                }
            };

            context.AddRange(pessoas);
            context.SaveChanges();

            pessoaService = new PessoaService(context);
        }

        // --- TESTES DE PROFESSOR ---

        [TestMethod()]
        public void CreateProfessorTest()
        {
            // Act
            var dto = new ProfessorPessoaDTO 
            { 
                Nome = "Professor Xavier", 
                Sobrenome = "X-Men",
                Cpf = "44444444444", 
                Email = "xavier@xmen.com",
                Senha = "123567",
                Telefone = "11966666666",
                Genero = "Masculino",
                DataNascimento = new DateTime(1960, 5, 10),
                DescricaoProfessor = "História",
                Libras = false,
                IdCidade = 101,
                // Dados Obrigatórios
                 Cep = "11111-111", Rua = "Rua X", Numero = "10", Complemento = "X bairo", Bairro = "Mutante", Estado = "US", Cidade = "NY",
            };

            pessoaService.CreateProfessor(dto);

            // Assert
            Assert.AreEqual(2, pessoaService.GetAllProfessores().Count());
            
            // Busca pelo nome para garantir que salvou
            var prof = pessoaService.GetProfessoresByNome("Professor Xavier").FirstOrDefault();
            Assert.IsNotNull(prof);
            Assert.AreEqual("História", prof.DescricaoProfessor);
        }

        [TestMethod()]
        public void EditProfessorTest()
        {
            // Act - Editando o ID 1 (Girafales)
            var dto = new ProfessorPessoaDTO
            {
                Id = 1,
                Nome = "Professor Linguiça", // Mudando nome
                Sobrenome = "Madruga",
                Email = "girafales@email.com",
                Telefone = "11999999999",
                Cep = "00000-000", Rua = "Rua A", Numero = "1", Bairro = "B", Cidade = "SP", Estado = "SP",
                DescricaoProfessor = "Português", // Mudando matéria
                Libras = true,
                IdCidade = 200,
                Senha = "123", //Não posso alterar a senha, pois meu dto retorna vazio
                Genero = "Masculino",
                Complemento =  "Casa Girafales",
            };

            pessoaService.EditProfessor(dto);

            // Assert
            var profEditado = pessoaService.GetProfessor(1);
            Assert.AreEqual("Professor Linguiça", profEditado.Nome);
            Assert.AreEqual("Português", profEditado.DescricaoProfessor);
            Assert.AreEqual(200, profEditado.IdCidade);
            Assert.AreEqual("Casa Girafales", profEditado.Complemento);
        }

        [TestMethod()]
        public void GetProfessorTest()
        {
            var prof = pessoaService.GetProfessor(1);
            Assert.IsNotNull(prof);
            Assert.AreEqual("Professor Girafales", prof.Nome);
            // Verifica se a senha foi limpa conforme regra de negócio do Get
            Assert.AreEqual("", prof.Senha); 
        }

        [TestMethod()]
        public void GetAllProfessoresTest()
        {
            var lista = pessoaService.GetAllProfessores();
            
            Assert.IsNotNull(lista);
            // Deve retornar 1 (apenas o Girafales), ignorando Chaves e Madruga
            Assert.AreEqual(1, lista.Count());
            Assert.AreEqual("Professor Girafales", lista.First().Nome);
        }

        [TestMethod()]
        public void GetProfessoresByNomeTest()
        {
            var lista = pessoaService.GetProfessoresByNome("Professor");
            Assert.AreEqual(1, lista.Count());
            Assert.AreEqual(1, lista.First().Id);
        }

        // --- TESTES DE ALUNO ---

        [TestMethod()]
        public void CreateAlunoTest()
        {
            // Act
            var dto = new AlunoPessoaDTO
            {
                Nome = "Kiko",
                Sobrenome = "Tesouro",
                Cpf = "55555555555",
                Email = "kiko@email.com",
                Senha = "123",
                AlunoDeMenor = true,
                Atipico = false,
                IdResponsavel = 3,
                Complemento = "Casa X",
                DataNascimento = new DateTime(2010, 06, 15),
                Telefone = "11955555555", Cep = "22222-222", Rua = "Rua K", Numero = "20", Bairro = "Vila", Cidade = "SP", Estado = "SP", Genero = "M"
            };

            pessoaService.CreateAluno(dto);

            // Assert
            Assert.AreEqual(2, pessoaService.GetAllAlunos().Count());
            var aluno = pessoaService.GetAlunosByNome("Kiko").FirstOrDefault();
            Assert.IsNotNull(aluno);
            Assert.AreEqual(true, aluno.AlunoDeMenor);
        }

        [TestMethod()]
        public void EditAlunoTest()
        {
            // Act - Editando ID 2 (Chaves)
            var dto = new AlunoPessoaDTO
            {
                Id = 2,
                Nome = "Chaves do 8",
                Sobrenome = "Ninguém sabe",
                Email = "chaves@email.com",
                Telefone = "11988888888",
                Cep = "00000-000", Rua = "Rua B", Numero = "2", Bairro = "B", Cidade = "SP", Estado = "SP",
                AlunoDeMenor = false, // Cresceu
                Atipico = true,
                IdResponsavel = 3
            };

            pessoaService.EditAluno(dto);

            // Assert
            var alunoEditado = pessoaService.GetAluno(2);
            Assert.AreEqual("Chaves do 8", alunoEditado.Nome);
            Assert.AreEqual(false, alunoEditado.AlunoDeMenor);
            Assert.AreEqual(true, alunoEditado.Atipico);
        }

        [TestMethod()]
        public void GetAlunoTest()
        {
            var aluno = pessoaService.GetAluno(2);
            Assert.IsNotNull(aluno);
            Assert.AreEqual("Chaves", aluno.Nome);
            Assert.AreEqual("", aluno.Senha); // Senha limpa
        }

        [TestMethod()]
        public void GetAllAlunosTest()
        {
            var lista = pessoaService.GetAllAlunos();
            Assert.IsNotNull(lista);
            // Deve retornar 1 (Apenas Chaves)
            Assert.AreEqual(1, lista.Count());
            Assert.AreEqual(2, lista.First().Id);
        }

        // --- TESTES DE RESPONSÁVEL ---

        [TestMethod()]
        public void CreateResponsavelTest()
        {
            // Act
            var dto = new ResponsavelPessoaDTO
            {
                Nome = "Dona Florinda",
                Sobrenome = "Corcuera",
                Cpf = "66666666666",
                Email = "florinda@email.com",
                Senha = "123",
                QuantidadeDeDependentes = 1,
                DataNascimento = new DateTime(1990, 01, 01),
                Complemento = "Casa X",
                Telefone = "11944444444", Cep = "33333-333", Rua = "Rua F", Numero = "30", Bairro = "Vila", Cidade = "SP", Estado = "SP", Genero = "F"
            };

            pessoaService.CreateResponsavel(dto);

            // Assert
            Assert.AreEqual(2, pessoaService.GetAllResponsaveis().Count());
            var resp = pessoaService.GetResponsaveisByNome("Dona").FirstOrDefault();
            Assert.IsNotNull(resp);
            Assert.AreEqual(1, resp.QuantidadeDeDependentes);
        }

        [TestMethod()]
        public void EditResponsavelTest()
        {
            // Act - Editando ID 3 (Seu Madruga)
            var dto = new ResponsavelPessoaDTO
            {
                Id = 3,
                Nome = "Don Ramón",
                Sobrenome = "Valdez",
                Email = "madruga@email.com",
                Telefone = "11977777777",
                Cep = "00000-000", Rua = "Rua C", Numero = "3", Bairro = "B", Cidade = "SP", Estado = "SP",
                QuantidadeDeDependentes = 3 // Aumentou a familia
            };

            pessoaService.EditResponsavel(dto);

            // Assert
            var respEditado = pessoaService.GetResponsavel(3);
            Assert.AreEqual("Don Ramón", respEditado.Nome);
            Assert.AreEqual(3, respEditado.QuantidadeDeDependentes);
        }

        [TestMethod()]
        public void GetResponsavelTest()
        {
            var resp = pessoaService.GetResponsavel(3);
            Assert.IsNotNull(resp);
            Assert.AreEqual("Seu Madruga", resp.Nome);
            Assert.AreEqual("", resp.Senha);
        }

        [TestMethod()]
        public void GetAllResponsaveisTest()
        {
            var lista = pessoaService.GetAllResponsaveis();
            Assert.IsNotNull(lista);
            // Deve retornar 1 (Apenas Seu Madruga)
            Assert.AreEqual(1, lista.Count());
            Assert.AreEqual(3, lista.First().Id);
        }

        // --- TESTES GENÉRICOS (DELETE) ---

        [TestMethod()]
        public void DeleteTest()
        {
            // Act - Deletando o Professor (ID 1)
            pessoaService.Delete(1);

            // Assert
            // Verifica se sumiu da lista geral
            Assert.AreEqual(2, context.Pessoas.Count()); // Eram 3, sobraram 2
            
            // Verifica se sumiu do GetAllProfessores
            Assert.AreEqual(0, pessoaService.GetAllProfessores().Count());

            // Tenta buscar e espera erro ou null (depende da implementação do Find no Get)
            // Como GetProfessor lança exceção se não achar:
            Assert.ThrowsException<ServiceException>(() => pessoaService.GetProfessor(1));
        }
        
        [TestMethod()]
        public void DeleteTest_IdInexistente()
        {
            // Act - Deletar algo que não existe não deve dar erro
            pessoaService.Delete(999);

            // Assert
            Assert.AreEqual(3, context.Pessoas.Count()); // Continua com 3
        }
    }
}