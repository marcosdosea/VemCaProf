using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var builder = new DbContextOptionsBuilder<VemCaProfContext>();
            builder.UseInMemoryDatabase("VCP" + Guid.NewGuid().ToString());
            var options = builder.Options;

            context = new VemCaProfContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var pessoas = new List<Pessoa>
            {
                // ID 1: Professor
                new Pessoa 
                { 
                    Id = 1, 
                    Nome = "Professor Girafales", 
                    Sobrenome = "Madruga",
                    Cpf = "11111111111", 
                    Email = "prof@email.com",
                    DescricaoProfessor = "Matemática",
                    TipoPessoa = "P", 
                    Libras = true,
                    IdCidade = 100,
                    Genero = "Masculino",
                    Complemento = "Casa X",
                    Telefone = "11999999999", Cep = "00000-000", Rua = "Rua A", Numero = "1", Bairro = "B", Cidade = "SP", Estado = "SP",
                    DataNascimento = new DateTime(1970, 1, 1)
                },

                // ID 2: Aluno
                new Pessoa 
                { 
                    Id = 2, 
                    Nome = "Chaves", 
                    Sobrenome = "do Oito",
                    Cpf = "22222222222", 
                    Email = "chaves@email.com",
                    AlunoDeMenor = true,
                    Atipico = false,
                    TipoPessoa = "A",
                    Genero = "Masculino",
                    Complemento = "Barril",
                    Telefone = "11988888888", Cep = "00000-000", Rua = "Rua B", Numero = "2", Bairro = "B", Cidade = "SP", Estado = "SP",
                    DataNascimento = new DateTime(2015, 5, 5)
                },

                // ID 3: Responsável
                new Pessoa 
                { 
                    Id = 3, 
                    Nome = "Seu Madruga", 
                    Sobrenome = "Ramón",
                    Cpf = "33333333333", 
                    Email = "madruga@email.com",
                    QuantidadeDeDependentes = 1,
                    TipoPessoa = "R",
                    Genero = "Masculino",
                    Complemento = "Vila",
                    Telefone = "11977777777", Cep = "00000-000", Rua = "Rua C", Numero = "3", Bairro = "B", Cidade = "SP", Estado = "SP",
                    DataNascimento = new DateTime(1980, 10, 10)
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
            var professor = new Pessoa 
            { 
                Nome = "Professor Xavier", 
                Sobrenome = "X-Men",
                Cpf = "44444444444", 
                Email = "xavier@xmen.com",
                DescricaoProfessor = "História",
                TipoPessoa = "P",
                Telefone = "11966666666", Cep = "11111-111", Rua = "Rua X", Numero = "10", Complemento = "X bairo", Bairro = "Mutante", Estado = "US", Cidade = "NY",
                Genero = "Masculino", Libras = false, IdCidade = 101, DataNascimento = new DateTime(1960, 5, 10)
            };

            pessoaService.Create(professor);
            
            var prof = pessoaService.GetByNome("Professor Xavier").FirstOrDefault();
            Assert.IsNotNull(prof);
            Assert.AreEqual("História", prof.DescricaoProfessor);
            Assert.AreEqual("X bairo", prof.Complemento);
        }

        [TestMethod()]
        public void EditProfessorTest()
        {
            var professor = context.Pessoas.Find(1)!;
            professor.Nome = "Professor Linguiça";
            professor.DescricaoProfessor = "Português";
            professor.Complemento = "Casa Girafales";
            professor.IdCidade = 200;

            pessoaService.Edit(professor);

            var profEditado = pessoaService.Get(1);
            Assert.AreEqual("Professor Linguiça", profEditado.Nome);
            Assert.AreEqual("Português", profEditado.DescricaoProfessor);
            Assert.AreEqual("Casa Girafales", profEditado.Complemento);
        }

        [TestMethod()]
        public void GetProfessorTest()
        {
            var prof = pessoaService.Get(1);
            Assert.IsNotNull(prof);
            Assert.AreEqual("Professor Girafales", prof.Nome);
            Assert.AreEqual("Casa X", prof.Complemento);
        }

        // --- TESTES DE ALUNO ---

        [TestMethod()]
        public void CreateAlunoTest()
        {
            var aluno = new Pessoa
            {
                Nome = "Kiko",
                Sobrenome = "Tesouro",
                Cpf = "55555555555",
                Email = "kiko@email.com",
                AlunoDeMenor = true,
                Atipico = false,
                TipoPessoa = "A",
                ResponsavelId = 3,
                Complemento = "Casa X",
                Genero = "Masculino",
                DataNascimento = new DateTime(2010, 06, 15),
                Telefone = "11955555555", Cep = "22222-222", Rua = "Rua K", Numero = "20", Bairro = "Vila", Cidade = "SP", Estado = "SP"
            };

            pessoaService.Create(aluno);

            var kiko = pessoaService.GetByNome("Kiko").FirstOrDefault();
            Assert.AreEqual("Casa X", kiko.Complemento);
        }

        [TestMethod()]
        public void EditAlunoTest()
        {
            var aluno = context.Pessoas.Find(2)!;
            aluno.Nome = "Chaves do 8";
            aluno.Atipico = true;
            aluno.AlunoDeMenor = false;

            pessoaService.Edit(aluno);

            var alunoEditado = pessoaService.Get(2);
            Assert.AreEqual("Chaves do 8", alunoEditado.Nome);
            Assert.IsTrue(alunoEditado.Atipico ?? false);
            Assert.IsFalse(alunoEditado.AlunoDeMenor ?? true);
        }

        // --- TESTES DE RESPONSÁVEL ---

        [TestMethod()]
        public void CreateResponsavelTest()
        {
            var responsavel = new Pessoa
            {
                Nome = "Dona Florinda",
                Sobrenome = "Corcuera",
                Cpf = "66666666666",
                Email = "florinda@email.com",
                QuantidadeDeDependentes = 1,
                TipoPessoa = "R",
                Complemento = "Casa X",
                Genero = "Feminino",
                DataNascimento = new DateTime(1990, 01, 01),
                Telefone = "11944444444", Cep = "33333-333", Rua = "Rua F", Numero = "30", Bairro = "Vila", Cidade = "SP", Estado = "SP"
            };

            pessoaService.Create(responsavel);

            Assert.AreEqual("R", responsavel.TipoPessoa);
        }

        [TestMethod()]
        public void EditResponsavelTest()
        {
            var responsavel = context.Pessoas.Find(3)!;
            responsavel.Nome = "Don Ramón";
            responsavel.QuantidadeDeDependentes = 3;
            responsavel.Sobrenome = "Valdez";

            pessoaService.Edit(responsavel);

            var respEditado = pessoaService.Get(3);
            Assert.AreEqual("Don Ramón", respEditado.Nome);
            Assert.AreEqual(3, respEditado.QuantidadeDeDependentes);
            Assert.AreEqual("Valdez", respEditado.Sobrenome);
        }

        // --- TESTE DE DELETE ---

        [TestMethod()]
        public void DeleteTest()
        {
            pessoaService.Delete(1);
            Assert.AreEqual(2, context.Pessoas.Count());
            Assert.ThrowsException<ServiceException>(() => pessoaService.Get(1));
        }
    }
}