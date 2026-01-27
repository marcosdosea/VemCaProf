using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Tests.Controllers
{
    [TestClass]
    public class DisciplinaControllerTests
    {
        // Método Helper para criar o Controller com Mocks
        private DisciplinaController CreateController(
            Mock<IDisciplinaService>? disciplinaServiceMock = null,
            Mock<IMapper>? mapperMock = null)
        {
            disciplinaServiceMock ??= new Mock<IDisciplinaService>();
            mapperMock ??= new Mock<IMapper>();

            return new DisciplinaController(disciplinaServiceMock.Object, mapperMock.Object);
        }

        [TestMethod]
        public void Index_RetornaViewComListaMapeada()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 1, Nome = "Matemática" };
            var disciplinas = new List<Disciplina> { disciplina };
            var disciplinaModel = new DisciplinaModel { Id = 1, Nome = "Matemática" };
            var mappedList = new List<DisciplinaModel> { disciplinaModel };

            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            disciplinaServiceMock.Setup(s => s.GetAll()).Returns(disciplinas);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<List<DisciplinaModel>>(It.IsAny<IEnumerable<Disciplina>>())).Returns(mappedList);

            var controller = CreateController(disciplinaServiceMock, mapperMock);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(mappedList, viewResult.Model);
        }

        [TestMethod]
        public void Details_RetornaViewComModeloMapeado()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 2, Nome = "Física" };
            var disciplinaModel = new DisciplinaModel { Id = 2, Nome = "Física" };

            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            disciplinaServiceMock.Setup(s => s.Get((uint)2)).Returns(disciplina);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<DisciplinaModel>(disciplina)).Returns(disciplinaModel);

            var controller = CreateController(disciplinaServiceMock, mapperMock);

            // Act
            var result = controller.Details(2);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(disciplinaModel, viewResult.Model);
        }

        [TestMethod]
        public void Create_Get_RetornaView()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Create_Post_ModelValido_ChamaCreateERedireciona()
        {
            // Arrange
            var disciplinaModel = new DisciplinaModel { Id = 3, Nome = "Química" };
            var disciplina = new Disciplina { Id = 3, Nome = "Química" };

            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            disciplinaServiceMock.Setup(s => s.Create(It.IsAny<Disciplina>())).Returns((uint)disciplina.Id);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Disciplina>(disciplinaModel)).Returns(disciplina);

            var controller = CreateController(disciplinaServiceMock, mapperMock);

            // Act
            var result = controller.Create(disciplinaModel);

            // Assert
            disciplinaServiceMock.Verify(s => s.Create(It.Is<Disciplina>(d => d == disciplina)), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
        }

        [TestMethod]
        public void Create_Post_ModelInvalido_NaoChamaCreateERedireciona()
        {
            // Arrange
            var disciplinaModel = new DisciplinaModel { Id = 4, Nome = "Bio" };
            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            var mapperMock = new Mock<IMapper>();

            var controller = CreateController(disciplinaServiceMock, mapperMock);
            controller.ModelState.AddModelError("Nome", "Erro de validação");

            // Act
            var result = controller.Create(disciplinaModel);

            // Assert
            disciplinaServiceMock.Verify(s => s.Create(It.IsAny<Disciplina>()), Times.Never);

            // OBSERVAÇÃO: Em MVC padrão, se o modelo é inválido, retorna-se a View, não um Redirect.
            // O código abaixo assume que seu controller retorna a View quando há erro.
            // Se o seu teste original exigia Redirect, troque typeof(ViewResult) por typeof(RedirectToActionResult).
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Edit_Get_RetornaViewComModeloMapeado()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 5, Nome = "História" };
            var disciplinaModel = new DisciplinaModel { Id = 5, Nome = "História" };

            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            disciplinaServiceMock.Setup(s => s.Get((uint)5)).Returns(disciplina);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<DisciplinaModel>(disciplina)).Returns(disciplinaModel);

            var controller = CreateController(disciplinaServiceMock, mapperMock);

            // Act
            var result = controller.Edit(5);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(disciplinaModel, viewResult.Model);
        }

        [TestMethod]
        public void Edit_Post_ModelValido_ChamaEditERedireciona()
        {
            // Arrange
            var disciplinaModel = new DisciplinaModel { Id = 6, Nome = "Geografia" };
            var disciplina = new Disciplina { Id = 6, Nome = "Geografia" };

            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Disciplina>(disciplinaModel)).Returns(disciplina);

            var controller = CreateController(disciplinaServiceMock, mapperMock);

            // Act
            var result = controller.Edit(6, disciplinaModel);

            // Assert
            disciplinaServiceMock.Verify(s => s.Edit(It.Is<Disciplina>(d => d == disciplina)), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
        }

        [TestMethod]
        public void Delete_Get_RetornaViewComModeloMapeado()
        {
            // Arrange
            var disciplina = new Disciplina { Id = 7, Nome = "Artes" };
            var disciplinaModel = new DisciplinaModel { Id = 7, Nome = "Artes" };

            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            disciplinaServiceMock.Setup(s => s.Get((uint)7)).Returns(disciplina);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<DisciplinaModel>(disciplina)).Returns(disciplinaModel);

            var controller = CreateController(disciplinaServiceMock, mapperMock);

            // Act
            var result = controller.Delete(7);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(disciplinaModel, viewResult.Model);
        }

        [TestMethod]
        public void Delete_Post_ChamaDeleteERedireciona()
        {
            // Arrange
            var disciplinaServiceMock = new Mock<IDisciplinaService>();
            var mapperMock = new Mock<IMapper>();

            var controller = CreateController(disciplinaServiceMock, mapperMock);

            // Cria um FormCollection vazio
            var form = new FormCollection(new Dictionary<string, StringValues>());

            // Act
            var result = controller.Delete(8, form);

            // Assert
            disciplinaServiceMock.Verify(s => s.Delete((uint)8), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
        }
    }
}