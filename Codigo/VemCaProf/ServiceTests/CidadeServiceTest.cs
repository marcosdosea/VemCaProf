using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Service.Tests
{
    [TestClass()]
    public class CidadeServiceTests
    {
        private VemCaProfContext context = null;
        private ICidadeService cidadeService = null;

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

            var cidades = new List<Cidade>
            {
                new() { Id = 1, Nome = "São Paulo", Estado = "SP" },
                new() { Id = 2, Nome = "Rio de Janeiro", Estado = "RJ" },
                new() { Id = 3, Nome = "Belo Horizonte", Estado = "MG" },
            };

            context.AddRange(cidades);
            context.SaveChanges();

            cidadeService = new CidadeService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            var novaCidade = new CidadeDTO
            {
                Nome = "Curitiba",
                Estado = "PR"
            };

            // Act
            var id = cidadeService.Create(novaCidade);

            // Assert
            Assert.AreEqual(4, cidadeService.GetAll().Count());
            var cidade = cidadeService.Get(id);
            Assert.IsNotNull(cidade);
            Assert.AreEqual("Curitiba", cidade.Nome);
            Assert.AreEqual("PR", cidade.Estado);
        }

        [TestMethod()]
        [ExpectedException(typeof(ServiceException))]
        public void CreateTest_NomeVazio_DeveLancarExcecao()
        {
            // Arrange
            var cidadeInvalida = new CidadeDTO
            {
                Nome = "",
                Estado = "SP"
            };

            // Act
            cidadeService.Create(cidadeInvalida);
        }

        [TestMethod()]
        [ExpectedException(typeof(ServiceException))]
        public void CreateTest_EstadoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var cidadeInvalida = new CidadeDTO
            {
                Nome = "Teste",
                Estado = "S" // Apenas 1 caractere
            };

            // Act
            cidadeService.Create(cidadeInvalida);
        }

        [TestMethod()]
        [ExpectedException(typeof(ServiceException))]
        public void CreateTest_CidadeDuplicada_DeveLancarExcecao()
        {
            // Arrange
            var cidadeDuplicada = new CidadeDTO
            {
                Nome = "São Paulo",
                Estado = "SP"
            };

            // Act
            cidadeService.Create(cidadeDuplicada);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            var resultado = cidadeService.Delete(2);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(2, cidadeService.GetAll().Count());
            Assert.IsNull(cidadeService.Get(2));
        }

        [TestMethod()]
        public void DeleteTest_IdInexistente_DeveRetornarFalse()
        {
            // Act
            var resultado = cidadeService.Delete(999);

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void EditTest()
        {
            // Arrange
            var cidadeDto = new CidadeDTO
            {
                Id = 3,
                Nome = "Belo Horizonte Atualizado",
                Estado = "MG"
            };

            // Act 
            var resultado = cidadeService.Update(cidadeDto);

            // Assert
            Assert.IsTrue(resultado);
            var cidadeAtualizada = cidadeService.Get(3);
            Assert.IsNotNull(cidadeAtualizada);
            Assert.AreEqual("Belo Horizonte Atualizado", cidadeAtualizada.Nome);
            Assert.AreEqual("MG", cidadeAtualizada.Estado);
        }

        [TestMethod()]
        public void EditTest_IdInexistente_DeveRetornarFalse()
        {
            // Arrange
            var cidade = new CidadeDTO
            {
                Id = 999,
                Nome = "Inexistente",
                Estado = "XX"
            };

            // Act 
            var resultado = cidadeService.Update(cidade);

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        [ExpectedException(typeof(ServiceException))]
        public void EditTest_CidadeDuplicada_DeveLancarExcecao()
        {
            // Arrange
            var cidadeAtualizada = new CidadeDTO
            {
                Id = 2, 
                Nome = "São Paulo",
                Estado = "SP"
            };

            // Act
            cidadeService.Update(cidadeAtualizada);
        }

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var cidade = cidadeService.Get(1);

            // Assert
            Assert.IsNotNull(cidade);
            Assert.AreEqual(1, cidade.Id);
            Assert.AreEqual("São Paulo", cidade.Nome);
            Assert.AreEqual("SP", cidade.Estado);
        }

        [TestMethod()]
        public void GetTest_IdInexistente_DeveRetornarNull()
        {
            // Act
            var cidade = cidadeService.Get(999);

            // Assert
            Assert.IsNull(cidade);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaCidades = cidadeService.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaCidades, typeof(IEnumerable<CidadeDTO>));
            Assert.IsNotNull(listaCidades);
            Assert.AreEqual(3, listaCidades.Count());
            Assert.AreEqual(1, listaCidades.First().Id);
            Assert.AreEqual("São Paulo", listaCidades.First().Nome);
            Assert.AreEqual("SP", listaCidades.First().Estado);
        }

        [TestMethod()]
        public void GetByNomeEstadoTest()
        {
            // Act
            var cidade = cidadeService.GetByNomeEstado("São Paulo", "SP");

            // Assert
            Assert.IsNotNull(cidade);
            Assert.AreEqual("São Paulo", cidade.Nome);
            Assert.AreEqual("SP", cidade.Estado);
        }

        [TestMethod()]
        public void GetByNomeEstadoTest_CaseInsensitive()
        {
            // Act
            var cidade1 = cidadeService.GetByNomeEstado("são paulo", "sp"); 
            var cidade2 = cidadeService.GetByNomeEstado("SÃO PAULO", "SP"); 

            // Assert
            Assert.IsNotNull(cidade1);
            Assert.IsNotNull(cidade2);
            Assert.AreEqual("São Paulo", cidade1.Nome);
            Assert.AreEqual("SP", cidade1.Estado);
            Assert.AreEqual("São Paulo", cidade2.Nome);
            Assert.AreEqual("SP", cidade2.Estado);
        }

        [TestMethod()]
        public void GetByNomeEstadoTest_CidadeInexistente_DeveRetornarNull()
        {
            // Act
            var cidade = cidadeService.GetByNomeEstado("Cidade Inexistente", "XX");

            // Assert
            Assert.IsNull(cidade);
        }

        [TestMethod()]
        public void CreateTest_EstadoEmMinusculo_DeveConverterParaMaiusculo()
        {
            // Arrange
            var novaCidade = new CidadeDTO
            {
                Nome = "Florianópolis",
                Estado = "sc" 
            };

            // Act
            var id = cidadeService.Create(novaCidade);

            // Assert
            var cidade = cidadeService.Get(id);
            Assert.IsNotNull(cidade);
            Assert.AreEqual("SC", cidade.Estado);
        }

        [TestMethod()]
        public void EditTest_EstadoEmMinusculo_DeveConverterParaMaiusculo()
        {
            // Arrange
            var cidadeDto = new CidadeDTO
            {
                Id = 1,
                Nome = "São Paulo",
                Estado = "sp" // Minúsculo
            };

            // Act 
            var resultado = cidadeService.Update(cidadeDto);

            // Assert
            Assert.IsTrue(resultado);
            var cidadeAtualizada = cidadeService.Get(1);
            Assert.AreEqual("SP", cidadeAtualizada.Estado); 
        }

        [TestMethod()]
        public void CreateTest_ComEspacos_DeveTrim()
        {
            // Arrange
            var novaCidade = new CidadeDTO
            {
                Nome = "  Porto Alegre  ",
                Estado = "  RS  " // Com espaços
            };

            // Act
            var id = cidadeService.Create(novaCidade);

            // Assert
            var cidade = cidadeService.Get(id);
            Assert.IsNotNull(cidade);
            Assert.AreEqual("Porto Alegre", cidade.Nome);
            Assert.AreEqual("RS", cidade.Estado);
        }

        [TestMethod()]
        public void EditTest_ComEspacos_DeveTrim()
        {
            // Arrange
            var cidadeDto = new CidadeDTO
            {
                Id = 2,
                Nome = "  Rio  ",
                Estado = "  rj  "
            };

            // Act 
            var resultado = cidadeService.Update(cidadeDto);

            // Assert
            Assert.IsTrue(resultado);
            var cidadeAtualizada = cidadeService.Get(2);
            Assert.AreEqual("Rio", cidadeAtualizada.Nome);
            Assert.AreEqual("RJ", cidadeAtualizada.Estado);
        }
    }
}