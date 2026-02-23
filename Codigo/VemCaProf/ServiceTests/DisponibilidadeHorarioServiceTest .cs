using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Service.Tests
{
    [TestClass()]
    public class DisponibilidadeHorarioServiceTests
    {
        private VemCaProfContext context = null;
        private IDisponibilidadeHorarioService disponibilidadeHorarioService = null;

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

            var disponibilidadeHorarios = new List<DisponibilidadeHorario>
            {
                new() { Id = 1, Dia = new DateTime(2023, 02, 10, 14, 0, 0), HorarioInicio = new TimeSpan (10,14,12), HorarioFim = new TimeSpan(14,1,14),IdProfessor =1 },
                new() { Id = 2, Dia = new DateTime(2025, 01, 9, 12, 0, 0), HorarioInicio = new TimeSpan (1,4,2), HorarioFim = new TimeSpan(2,1,14),IdProfessor =2 },
                new() { Id = 3, Dia = new DateTime(2024, 03, 8, 13, 0, 0), HorarioInicio = new TimeSpan (2,14,12), HorarioFim = new TimeSpan(3,1,14),IdProfessor =3 },
            };

            context.AddRange(disponibilidadeHorarios);
            context.SaveChanges();

            disponibilidadeHorarioService = new DisponibilidadeHorarioService(context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            var novaDisponibilidadeHorario = new DisponibilidadeHorarioDTO
            {
                
                Dia = new DateTime(2026, 02, 10, 14, 0, 0),
                HorarioInicio = new TimeSpan(10, 14, 12),
                HorarioFim = new TimeSpan(14, 1, 14),
                IdProfessor = 1
            };
            

            // Act
            var id = disponibilidadeHorarioService.Create(novaDisponibilidadeHorario);

            // Assert
            Assert.AreEqual(4, disponibilidadeHorarioService.GetAll().Count());
            var disponibilidadeHorario = disponibilidadeHorarioService.Get(id);
            Assert.IsNotNull(disponibilidadeHorario);
            Assert.AreEqual(new DateTime(2026, 02, 10, 14, 0, 0), disponibilidadeHorario.Dia);
            Assert.AreEqual(new TimeSpan(10, 14, 12), disponibilidadeHorario.HorarioInicio);
            Assert.AreEqual(new TimeSpan(14, 1, 14), disponibilidadeHorario.HorarioFim);
            Assert.AreEqual( 1, disponibilidadeHorario.IdProfessor);
        }

        [TestMethod()]
        [ExpectedException(typeof(ServiceException))]
        public void CreateTest_DiaVazio_DeveLancarExcecao()
        {
            // Arrange
            var disponibilidadeHorarioInvalida = new DisponibilidadeHorarioDTO
            {
                Dia = DateTime.MinValue,
                HorarioInicio = new TimeSpan(10, 14, 12),
                HorarioFim = new TimeSpan(14, 1, 14),
                IdProfessor = 1

            };

            // Act
            disponibilidadeHorarioService.Create(disponibilidadeHorarioInvalida);
        }


        
        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            var resultado = disponibilidadeHorarioService.Delete(2);

            // Assert
            Assert.IsTrue(resultado);
            Assert.AreEqual(2, disponibilidadeHorarioService.GetAll().Count());
            Assert.IsNull(disponibilidadeHorarioService.Get(2));
        }

        [TestMethod()]
        public void DeleteTest_IdInexistente_DeveRetornarFalse()
        {
            // Act
            var resultado = disponibilidadeHorarioService.Delete(999);

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod()]
        public void EditTest()
        {
            // Arrange
            var disponibilidadeHorarioDto = new DisponibilidadeHorarioDTO
            {
                Id = 3,
                Dia = new DateTime(2027, 03, 8, 13, 0, 0),
                HorarioInicio = new TimeSpan(2, 14, 12),
                HorarioFim = new TimeSpan(3, 1, 14),
                IdProfessor = 4
                
            };

            // Act 
            var resultado = disponibilidadeHorarioService.Update(disponibilidadeHorarioDto);

            // Assert
            Assert.IsTrue(resultado);
            var disponibilidadeHorarioAtualizada = disponibilidadeHorarioService.Get(3);
            Assert.IsNotNull(disponibilidadeHorarioAtualizada);
            Assert.AreEqual(new DateTime(2027, 03, 8, 13, 0, 0), disponibilidadeHorarioAtualizada.Dia);
            Assert.AreEqual(4, disponibilidadeHorarioAtualizada.IdProfessor);
        }

        [TestMethod()]
        public void EditTest_IdInexistente_DeveRetornarFalse()
        {
            // Arrange
            var disponibilidadeHorario = new DisponibilidadeHorarioDTO
            {
                Id = 999,
                Dia = new DateTime(2027, 03, 8, 13, 0, 0),
                HorarioInicio = new TimeSpan(2, 14, 12),
                HorarioFim = new TimeSpan(3, 1, 14),
                IdProfessor = 4

            };

            // Act 
            var resultado = disponibilidadeHorarioService.Update(disponibilidadeHorario);

            // Assert
            Assert.IsFalse(resultado);
        }

      

        [TestMethod()]
        public void GetTest()
        {
            // Act
            var disponibilidadeHorario = disponibilidadeHorarioService.Get(1);

            // Assert
            Assert.IsNotNull(disponibilidadeHorario);
            Assert.AreEqual(1, disponibilidadeHorario.Id);
            Assert.AreEqual(new DateTime(2023, 02, 10, 14, 0, 0), disponibilidadeHorario.Dia);
            Assert.AreEqual(new TimeSpan(10, 14, 12), disponibilidadeHorario.HorarioInicio);
            Assert.AreEqual(new TimeSpan(14, 1, 14), disponibilidadeHorario.HorarioFim);
            Assert.AreEqual(1, disponibilidadeHorario.IdProfessor);
        }

        [TestMethod()]
        public void GetTest_IdInexistente_DeveRetornarNull()
        {
            // Act
            var disponibilidadeHorario = disponibilidadeHorarioService.Get(999);

            // Assert
            Assert.IsNull(disponibilidadeHorario);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaDisponibilidadeHorarios = disponibilidadeHorarioService.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaDisponibilidadeHorarios, typeof(IEnumerable<DisponibilidadeHorarioDTO>));
            Assert.IsNotNull(listaDisponibilidadeHorarios);
            Assert.AreEqual(3, listaDisponibilidadeHorarios.Count());
            Assert.AreEqual(1, listaDisponibilidadeHorarios.First().Id);
            Assert.AreEqual(new DateTime(2023, 02, 10, 14, 0, 0), listaDisponibilidadeHorarios.First().Dia);
            Assert.AreEqual(new TimeSpan(10, 14, 12), listaDisponibilidadeHorarios.First().HorarioInicio);
            Assert.AreEqual(new TimeSpan(14, 1, 14), listaDisponibilidadeHorarios.First().HorarioFim);
            Assert.AreEqual(1, listaDisponibilidadeHorarios.First().IdProfessor); 
        }

    }
}