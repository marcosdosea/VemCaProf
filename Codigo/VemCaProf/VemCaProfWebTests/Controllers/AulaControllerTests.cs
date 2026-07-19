using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers.Tests
{
    [TestClass()]
    public class AulaControllerTests
    {
        private AulaController controller;
        private Mock<IAulaService> mockService;
        private Mock<IDisciplinaService> mockDisciplinaService;
        private Mock<IPessoaService> mockPessoaService;
        private Mock<ILogger<AulaController>> mockLogger;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            mockService = new Mock<IAulaService>();
            mockDisciplinaService = new Mock<IDisciplinaService>();
            mockPessoaService = new Mock<IPessoaService>();
            mockLogger = new Mock<ILogger<AulaController>>();

            IMapper mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AulaDTO, AulaModel>().ReverseMap();
            }).CreateMapper();

            // Configuração dos Mocks (Stubs)
            mockService.Setup(service => service.GetAll())
                .Returns(GetTestAulasDTO());

            mockService.Setup(service => service.Get(It.IsAny<int>()))
                .Returns(GetTargetAulaDTO());

            mockService.Setup(service => service.Create(It.IsAny<AulaDTO>()))
                .Verifiable();

            mockService.Setup(service => service.Update(It.IsAny<AulaDTO>()))
                .Returns(true)
                .Verifiable();

            mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Returns(true)
                .Verifiable();

            mockService.Setup(service => service.CancelarAula(It.IsAny<int>()))
                .Verifiable();

            mockService.Setup(service => service.ConfirmarAula(It.IsAny<int>()))
                .Verifiable();
            mockService.Setup(service => service.GetHorariosDisponiveis(
                    It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int?>()))
                .Returns(Array.Empty<DisponibilidadeHorarioDTO>());

            mockDisciplinaService.Setup(service => service.GetAll())
                .Returns(new[] { new Disciplina { Id = 1, Nome = "Matemática", Nivel = "F2" } });
            mockPessoaService.Setup(service => service.GetAll())
                .Returns(new[]
                {
                    new Pessoa { Id = 1, Nome = "Paulo", Sobrenome = "Professor", Email = "paulo@teste.com", TipoPessoa = "P" },
                    new Pessoa { Id = 2, Nome = "Rita", Sobrenome = "Responsável", Email = "rita@teste.com", TipoPessoa = "R" },
                    new Pessoa { Id = 3, Nome = "Alice", Sobrenome = "Aluna", Email = "alice@teste.com", TipoPessoa = "A" }
                });

            controller = new AulaController(
                mockService.Object,
                mockDisciplinaService.Object,
                mockPessoaService.Object,
                mapper,
                mockLogger.Object);

            // Mock do TempData necessário para a AulaController não quebrar
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
        }

        [TestMethod()]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<AulaModel>));

            List<AulaModel>? lista = ((IEnumerable<AulaModel>)viewResult.ViewData.Model).ToList();
            Assert.AreEqual(3, lista.Count);
            Assert.AreEqual("Aula de Matemática Básica", lista.First().Descricao);
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AulaModel));

            AulaModel model = (AulaModel)viewResult.ViewData.Model;

            // Validações
            Assert.AreEqual("Aula de Matemática Básica", model.Descricao);
            Assert.AreEqual(50.0, model.Valor);
            Assert.AreEqual(1, model.Id);
        }

        [TestMethod()]
        public void CreateTest_Get_Valido()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Matemática - F2", ((SelectList)controller.ViewBag.Disciplinas).First().Text);
            Assert.AreEqual("Paulo Professor - paulo@teste.com", ((SelectList)controller.ViewBag.Professores).First().Text);
            Assert.AreEqual("Rita Responsável - rita@teste.com", ((SelectList)controller.ViewBag.Responsaveis).First().Text);
            Assert.AreEqual("Alice Aluna - alice@teste.com", ((SelectList)controller.ViewBag.Alunos).First().Text);
        }

        [TestMethod()]
        public void CreateTest_Post_Valid()
        {
            // Act
            var result = controller.Create(GetNewAulaModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Aula cadastrada com sucesso!", controller.TempData["SuccessMessage"]);
        }

        [TestMethod]
        public void CreateTest_PostInvalido_RecarregaOpcoes()
        {
            controller.ModelState.AddModelError("IdProfessor", "Professor inválido");

            var result = controller.Create(GetNewAulaModel());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Matemática - F2", ((SelectList)controller.ViewBag.Disciplinas).Single().Text);
            Assert.AreEqual("Paulo Professor - paulo@teste.com", ((SelectList)controller.ViewBag.Professores).Single().Text);
        }

        [TestMethod()]
        public void EditTest_Get_Valid()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AulaModel));

            AulaModel model = (AulaModel)viewResult.ViewData.Model;
            Assert.AreEqual("Aula de Matemática Básica", model.Descricao);
        }

        [TestMethod()]
        public void EditTest_Post_Valid()
        {
            // Act
            var result = controller.Edit(1, GetTargetAulaModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Aula atualizada com sucesso!", controller.TempData["SuccessMessage"]);
        }

        [TestMethod()]
        public void DeleteTest_Get_Valid()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AulaModel));

            AulaModel model = (AulaModel)viewResult.ViewData.Model;
            Assert.AreEqual("Aula de Matemática Básica", model.Descricao);
        }

        [TestMethod()]
        public void DeleteTest_Post_Valid()
        {
            // Act
            var result = controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Aula excluída com sucesso!", controller.TempData["SuccessMessage"]);
        }

        [TestMethod()]
        public void CancelarTest_Valid()
        {
            // Act
            var result = controller.Cancelar(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Aula cancelada com sucesso!", controller.TempData["SuccessMessage"]);
        }

        [TestMethod()]
        public void ConfirmarTest_Valid()
        {
            // Act
            var result = controller.Confirmar(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Participação confirmada com sucesso!", controller.TempData["SuccessMessage"]);
        }

        [TestMethod]
        public void HorariosDisponiveis_RetornaIdETextoFormatado()
        {
            var dataAula = new DateTime(2026, 7, 20);
            mockService.Setup(service => service.GetHorariosDisponiveis(1, dataAula, null))
                .Returns(new[]
                {
                    new DisponibilidadeHorarioDTO
                    {
                        Id = 8,
                        IdProfessor = 1,
                        Dia = new DateTime(2026, 7, 20),
                        HorarioInicio = TimeSpan.FromHours(9),
                        HorarioFim = TimeSpan.FromHours(10)
                    }
                });

            var result = controller.HorariosDisponiveis(1, dataAula);

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            var json = JsonSerializer.Serialize(((JsonResult)result).Value);
            StringAssert.Contains(json, "\"id\":8");
            StringAssert.Contains(json, "09:00");
            StringAssert.Contains(json, "10:00");
        }

        // --- Helper Methods ---

        private AulaModel GetNewAulaModel()
        {
            return new AulaModel
            {
                Id = 4,
                Descricao = "Aula de Física Moderna",
                Valor = 80.0,
                DataHorarioInicio = DateTime.Now.AddDays(1),
                DataHorarioFinal = DateTime.Now.AddDays(1).AddHours(1),
                IdDisponibilidadeHorario = 1,
                IdProfessor = 1,
                IdDisciplina = 1,
                IdResponsavel = 2,
                IdAluno = 3
            };
        }

        private static AulaDTO GetTargetAulaDTO()
        {
            return new AulaDTO
            {
                Id = 1,
                Descricao = "Aula de Matemática Básica",
                Valor = 50.0,
                DataHorarioInicio = DateTime.Now,
                DataHorarioFinal = DateTime.Now.AddHours(1),
                IdDisponibilidadeHorario = 1,
                IdProfessor = 1,
                IdDisciplina = 1,
                IdResponsavel = 2,
                IdAluno = 3
            };
        }

        private AulaModel GetTargetAulaModel()
        {
            return new AulaModel
            {
                Id = 1,
                Descricao = "Aula de Matemática Básica",
                Valor = 50.0,
                DataHorarioInicio = DateTime.Now,
                DataHorarioFinal = DateTime.Now.AddHours(1),
                IdDisponibilidadeHorario = 1,
                IdProfessor = 1,
                IdDisciplina = 1,
                IdResponsavel = 2,
                IdAluno = 3
            };
        }

        private List<AulaDTO> GetTestAulasDTO()
        {
            return new List<AulaDTO>
            {
                new AulaDTO
                {
                    Id = 1,
                    Descricao = "Aula de Matemática Básica",
                    Valor = 50.0,
                    DataHorarioInicio = DateTime.Now,
                    DataHorarioFinal = DateTime.Now.AddHours(1)
                },
                new AulaDTO
                {
                    Id = 2,
                    Descricao = "Aula de Química Orgânica",
                    Valor = 70.0,
                    DataHorarioInicio = DateTime.Now.AddDays(1),
                    DataHorarioFinal = DateTime.Now.AddDays(1).AddHours(1)
                },
                new AulaDTO
                {
                    Id = 3,
                    Descricao = "Aula de Biologia Celular",
                    Valor = 60.0,
                    DataHorarioInicio = DateTime.Now.AddDays(2),
                    DataHorarioFinal = DateTime.Now.AddDays(2).AddHours(1)
                }
            };
        }
    }
}
