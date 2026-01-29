using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Service.Tests
{
    [TestClass()]
    public class DisciplinaServiceTests
    {
        private VemCaProfContext context = null;
        private IDisciplinaService disciplinaService = null;

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

            var disciplinas = new List<Disciplina>
                {
                    new() { Id = 1, Nome = "Matemática" },
                    new() { Id = 2, Nome = "Português" },
                    new() { Id = 3, Nome = "História" },
                };

            context.AddRange(disciplinas);
            context.SaveChanges();

            disciplinaService = new DisciplinaService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            disciplinaService.Create(new Disciplina() { Id = 4, Nome = "Geografia" });

            // Assert
            Assert.AreEqual(4, disciplinaService.GetAll().Count());
            var disciplina = disciplinaService.Get(4);
            Assert.AreEqual("Geografia", disciplina.Nome);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            disciplinaService.Delete(2);

            // Assert
            Assert.AreEqual(2, disciplinaService.GetAll().Count());

            Assert.ThrowsException<ServiceException>(() => disciplinaService.Get(2));
        }

        [TestMethod()]
        public void EditTest()
        {
            // Act 
            var disciplina = disciplinaService.Get(3); 
            disciplina.Nome = "História da Arte";
            disciplinaService.Edit(disciplina);

            // Assert
            disciplina = disciplinaService.Get(3);
            Assert.IsNotNull(disciplina);
            Assert.AreEqual("História da Arte", disciplina.Nome);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var disciplina = disciplinaService.Get(1);

            // Assert
            Assert.IsNotNull(disciplina);
            Assert.AreEqual("Matemática", disciplina.Nome);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaDisciplina = disciplinaService.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaDisciplina, typeof(IEnumerable<Disciplina>));
            Assert.IsNotNull(listaDisciplina);
            Assert.AreEqual(3, listaDisciplina.Count());
            Assert.AreEqual(1, listaDisciplina.First().Id);
            Assert.AreEqual("Matemática", listaDisciplina.First().Nome);
        }

        [TestMethod()]
        public void GetByNomeTest()
        {
            // Act
            var disciplinas = disciplinaService.GetByNome("Mate");

            // Assert
            Assert.IsNotNull(disciplinas);
            Assert.AreEqual(1, disciplinas.Count());
            Assert.AreEqual("Matemática", disciplinas.First().Nome);
        }
    }
}