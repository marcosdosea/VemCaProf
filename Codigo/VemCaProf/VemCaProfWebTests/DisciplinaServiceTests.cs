using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using Core.Service; // Namespace onde está ServiceException e IDisciplinaService
using Service;      // Namespace da classe DisciplinaService
using System;
using System.Linq;

namespace Service.Tests
{
    [TestClass]
    public class DisciplinaServiceTests
    {
        private VemCaProfContext _context;
        private DisciplinaService _service;

        [TestInitialize]
        public void Initialize()
        {
            // Configura um banco de dados em memória único para cada teste
            // Guid.NewGuid() garante que um teste não interfira no outro
            var options = new DbContextOptionsBuilder<VemCaProfContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VemCaProfContext(options);
            _service = new DisciplinaService(_context);
        }

        [TestMethod]
        public void CreateTest_DeveSalvarDisciplina()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };

            // Act
            _service.Create(disciplina);

            // Assert
            var qtd = _context.Disciplinas.Count();
            var salvo = _context.Disciplinas.FirstOrDefault();

            Assert.AreEqual(1, qtd);
            Assert.IsNotNull(salvo);
            Assert.AreEqual("Matemática", salvo.Nome);
        }

        [TestMethod]
        public void DeleteTest_DeveRemoverDisciplinaExistente()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 1, Nome = "História" };
            _context.Disciplinas.Add(disciplina);
            _context.SaveChanges();

            // Act
            _service.Delete(1); // Passando uint

            // Assert
            Assert.AreEqual(0, _context.Disciplinas.Count());
        }

        [TestMethod]
        public void DeleteTest_IdNaoExistente_NaoDeveDarErro()
        {
            // Arrange
            // Banco vazio

            // Act
            _service.Delete(99);

            // Assert
            // Se chegou aqui sem exceção, o teste passou.
            // O seu método Delete apenas verifica "if (disciplina != null)", 
            // então não deve lançar erro se não achar.
            Assert.AreEqual(0, _context.Disciplinas.Count());
        }

        [TestMethod]
        public void EditTest_DadosValidos_DeveAtualizar()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 1, Nome = "Geografia" };
            _context.Disciplinas.Add(disciplina);
            _context.SaveChanges();

            // Desanexar para simular um objeto vindo da tela (Controller)
            _context.Entry(disciplina).State = EntityState.Detached;

            var disciplinaEditada = new Disciplina { Id = 1, Nome = "Geografia Avançada" };

            // Act
            _service.Edit(disciplinaEditada);

            // Assert
            var noBanco = _context.Disciplinas.Find(1);
            Assert.AreEqual("Geografia Avançada", noBanco.Nome);
        }

        [TestMethod]
        public void EditTest_IdZero_DeveLancarServiceException()
        {
            // Arrange
            var disciplinaInvalida = new Disciplina { Id = 0, Nome = "Erro" };

            // Act & Assert
            // Verifica se o seu código lança a ServiceException como programado
            var ex = Assert.ThrowsException<ServiceException>(() => _service.Edit(disciplinaInvalida));
            Assert.AreEqual("Disciplina inválida.", ex.Message);
        }

        [TestMethod]
        public void GetTest_IdExistente_DeveRetornarDisciplina()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 5, Nome = "Artes" };
            _context.Disciplinas.Add(disciplina);
            _context.SaveChanges();

            // Act
            var result = _service.Get(5);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Artes", result.Nome);
        }

        [TestMethod]
        public void GetTest_IdNaoExistente_DeveLancarServiceException()
        {
            // Arrange
            // Banco vazio

            // Act & Assert
            var ex = Assert.ThrowsException<ServiceException>(() => _service.Get(99));
            Assert.AreEqual("Disciplina não encontrada.", ex.Message);
        }

        [TestMethod]
        public void GetAllTest_DeveRetornarTodas()
        {
            // Arrange
            _context.Disciplinas.Add(new Disciplina { Id = 1, Nome = "A" });
            _context.Disciplinas.Add(new Disciplina { Id = 2, Nome = "B" });
            _context.SaveChanges();

            // Act
            var lista = _service.GetAll();

            // Assert
            Assert.AreEqual(2, lista.Count());
        }

        [TestMethod]
        public void GetByNomeTest_DeveFiltrarCorretamente()
        {
            // Arrange
            _context.Disciplinas.Add(new Disciplina { Id = 1, Nome = "Matemática" });
            _context.Disciplinas.Add(new Disciplina { Id = 2, Nome = "Matemática Financeira" });
            _context.Disciplinas.Add(new Disciplina { Id = 3, Nome = "História" });
            _context.SaveChanges();

            // Act
            // O seu método usa StartsWith, então "Mat" deve trazer as duas primeiras
            var resultado = _service.GetByNome("Mat");

            // Assert
            Assert.AreEqual(2, resultado.Count());
            Assert.IsTrue(resultado.All(d => d.Nome.Contains("Matemática")));
        }
    }
}