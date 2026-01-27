using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System.Collections.Generic;
using System.Linq;

namespace Service.Tests
{
    [TestClass()]
    public class DisciplinaServiceTests
    {
        private VemCaProfContext context;
        private IDisciplinaService disciplinaService;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<VemCaProfContext>();
            builder.UseInMemoryDatabase("VemCaProf");
            var options = builder.Options;

            context = new VemCaProfContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var disciplinas = new List<Disciplina>
                {
                    new Disciplina { Id = 1, Nome = "Matemática" },
                    new Disciplina { Id = 2, Nome = "Português" },
                    new Disciplina { Id = 3, Nome = "História" },
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

            // Obs: Como o seu método Get lança ServiceException quando não encontra,
            // usamos o Assert.ThrowsException em vez de verificar se é null.
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
            Assert.AreEqual(1, disciplina.Id);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaDisciplinas = disciplinaService.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaDisciplinas, typeof(IEnumerable<Disciplina>));
            Assert.IsNotNull(listaDisciplinas);
            Assert.AreEqual(3, listaDisciplinas.Count());
            Assert.AreEqual(1, listaDisciplinas.First().Id);
            Assert.AreEqual("Matemática", listaDisciplinas.First().Nome);
        }

        [TestMethod()]
        public void GetByNomeTest()
        {
            // Act
            var disciplinas = disciplinaService.GetByNome("Matemática");

            // Assert
            Assert.IsNotNull(disciplinas);
            Assert.AreEqual(1, disciplinas.Count());
            Assert.AreEqual("Matemática", disciplinas.First().Nome);
        }
    }
}