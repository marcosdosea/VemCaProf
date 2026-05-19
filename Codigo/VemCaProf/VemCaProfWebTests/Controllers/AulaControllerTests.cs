using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers.Tests
{
    [TestClass()]
    public class AulaControllerTests
    {
        private AulaController controller;
        private Mock<IAulaService> mockService;
        private Mock<ILogger<AulaController>> mockLogger;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            mockService = new Mock<IAulaService>();
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

            controller = new AulaController(mockService.Object, mapper, mockLogger.Object);

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
            Assert.AreEqual(50.0m, model.Valor);
            Assert.AreEqual(1, model.Id);
        }

        [TestMethod()]
        public void CreateTest_Get_Valido()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
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

        // --- Helper Methods ---

        private AulaModel GetNewAulaModel()
        {
            return new AulaModel
            {
                Id = 4,
                Descricao = "Aula de Física Moderna",
                Valor = 80.0m,
                DataHorarioInicio = DateTime.Now.AddDays(1),
                DataHorarioFinal = DateTime.Now.AddDays(1).AddHours(1)
            };
        }

        private static AulaDTO GetTargetAulaDTO()
        {
            return new AulaDTO
            {
                Id = 1,
                Descricao = "Aula de Matemática Básica",
                Valor = 50.0m,
                DataHorarioInicio = DateTime.Now,
                DataHorarioFinal = DateTime.Now.AddHours(1)
            };
        }

        private AulaModel GetTargetAulaModel()
        {
            return new AulaModel
            {
                Id = 1,
                Descricao = "Aula de Matemática Básica",
                Valor = 50.0