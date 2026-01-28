using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers.Tests
{
    [TestClass()]
    public class DisciplinaControllerTests
    {
        private static DisciplinaController controller;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var mockService = new Mock<IDisciplinaService>();

            IMapper mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Disciplina, DisciplinaModel>().ReverseMap();
            }).CreateMapper();

            // Configuração dos Mocks (Stubs)
            mockService.Setup(service => service.GetAll())
                .Returns(GetTestDisciplinas());

            mockService.Setup(service => service.Get(It.IsAny<uint>()))
                .Returns(GetTargetDisciplina());

            mockService.Setup(service => service.Create(It.IsAny<Disciplina>()))
                .Verifiable();

            mockService.Setup(service => service.Edit(It.IsAny<Disciplina>()))
                .Verifiable();

            mockService.Setup(service => service.Delete(It.IsAny<uint>()))
                .Verifiable();

            controller = new DisciplinaController(mockService.Object, mapper);
        }

        [TestMethod()]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<DisciplinaModel>));

            List<DisciplinaModel>? lista = (List<DisciplinaModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count);
            Assert.AreEqual("Matemática", lista.First().Nome);
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(DisciplinaModel));

            DisciplinaModel model = (DisciplinaModel)viewResult.ViewData.Model;

            // Validações
            Assert.AreEqual("Matemática", model.Nome);
            Assert.AreEqual("Cálculos e Lógica Fundamental", model.Descricao);
            Assert.AreEqual((uint)1, model.Id);
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
            var result = controller.Create(GetNewDisciplinaModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void EditTest_Get_Valid()
        {
            // Act
            var result = controller.Edit(0, 1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(DisciplinaModel));

            DisciplinaModel model = (DisciplinaModel)viewResult.ViewData.Model;
            Assert.AreEqual("Matemática", model.Nome);
            Assert.AreEqual("Cálculos e Lógica Fundamental", model.Descricao);
        }

        [TestMethod()]
        public void EditTest_Post_Valid()
        {
            // Act
            var result = controller.Edit(1, GetTargetDisciplinaModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void DeleteTest_Get_Valid()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(DisciplinaModel));

            DisciplinaModel model = (DisciplinaModel)viewResult.ViewData.Model;
            Assert.AreEqual("Matemática", model.Nome);
        }

        [TestMethod()]
        public void DeleteTest_Post_Valid()
        {
            // Act
            var result = controller.Delete(1, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private DisciplinaModel GetNewDisciplinaModel()
        {
            return new DisciplinaModel
            {
                Id = 4,
                Nome = "Geografia",
                Descricao = "Estudo da superfície terrestre"
            };
        }

        private static Disciplina GetTargetDisciplina()
        {
            return new Disciplina
            {
                Id = 1,
                Nome = "Matemática",
                Descricao = "Cálculos e Lógica Fundamental"
            };
        }

        private DisciplinaModel GetTargetDisciplinaModel()
        {
            return new DisciplinaModel
            {
                Id = 1,
                Nome = "Matemática",
                Descricao = "Cálculos e Lógica Fundamental"
            };
        }

        private IEnumerable<Disciplina> GetTestDisciplinas()
        {
            return new List<Disciplina>
            {
                new Disciplina {
                    Id = 1,
                    Nome = "Matemática",
                    Descricao = "Cálculos e Lógica Fundamental"
                },
                new Disciplina {
                    Id = 2,
                    Nome = "Português",
                    Descricao = "Gramática e Literatura"
                },
                new Disciplina {
                    Id = 3,
                    Nome = "História",
                    Descricao = "História Geral e do Brasil"
                },
            };
        }

    }
}