using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Tests.Controllers
{
    [TestClass]
    public class CidadeControllerTests
    {
        // Método Helper para criar o Controller com Mocks
        private CidadeController CreateController(
            Mock<ICidadeService>? cidadeServiceMock = null,
            Mock<IMapper>? mapperMock = null,
            Mock<ILogger<CidadeController>>? loggerMock = null)
        {
            cidadeServiceMock ??= new Mock<ICidadeService>();
            mapperMock ??= new Mock<IMapper>();
            loggerMock ??= new Mock<ILogger<CidadeController>>();

            return new CidadeController(cidadeServiceMock.Object, mapperMock.Object, loggerMock.Object);
        }

        [TestMethod]
        public void Index_RetornaViewComListaMapeada()
        {
            // Arrange
            var cidadesDto = new List<CidadeDTO>
            {
                new CidadeDTO { Id = 1, Nome = "São Paulo", Estado = "SP" },
                new CidadeDTO { Id = 2, Nome = "Rio de Janeiro", Estado = "RJ" }
            };

            var cidadesModel = new List<CidadeModel>
            {
                new CidadeModel { Id = 1, Nome = "São Paulo", Estado = "SP" },
                new CidadeModel { Id = 2, Nome = "Rio de Janeiro", Estado = "RJ" }
            };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.GetAll()).Returns(cidadesDto);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<CidadeModel>>(It.IsAny<IEnumerable<CidadeDTO>>()))
                     .Returns(cidadesModel);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(cidadesModel, viewResult.Model);
        }

        [TestMethod]
        public void Index_ErroNoService_RedirecionaParaHome()
        {
            // Arrange
            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.GetAll()).Throws(new Exception("Erro no serviço"));

            var mapperMock = new Mock<IMapper>();
            var loggerMock = new Mock<ILogger<CidadeController>>();

            var controller = CreateController(cidadeServiceMock, mapperMock, loggerMock);

            // Simular TempData
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Home", redirectResult.ControllerName);
            Assert.IsTrue(controller.TempData.ContainsKey("ErrorMessage"));
        }

        [TestMethod]
        public void Details_IdNulo_RetornaNotFound()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = controller.Details(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Details_IdExistente_RetornaViewComModeloMapeado()
        {
            // Arrange
            var cidadeDto = new CidadeDTO { Id = 1, Nome = "São Paulo", Estado = "SP" };
            var cidadeModel = new CidadeModel { Id = 1, Nome = "São Paulo", Estado = "SP" };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Get(1)).Returns(cidadeDto);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CidadeModel>(cidadeDto)).Returns(cidadeModel);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(cidadeModel, viewResult.Model);
        }

        [TestMethod]
        public void Details_IdInexistente_RetornaNotFound()
        {
            // Arrange
            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Get(999)).Returns((CidadeDTO?)null);

            var controller = CreateController(cidadeServiceMock);

            // Act
            var result = controller.Details(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
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
            var cidadeModel = new CidadeModel { Nome = "São Paulo", Estado = "SP" };
            var cidadeDto = new CidadeDTO { Nome = "São Paulo", Estado = "SP" };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Create(It.IsAny<CidadeDTO>())).Returns(1);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CidadeDTO>(cidadeModel)).Returns(cidadeDto);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Configurar TempData
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            // Act
            var result = controller.Create(cidadeModel);

            // Assert
            cidadeServiceMock.Verify(s => s.Create(It.Is<CidadeDTO>(d => d.Nome == "São Paulo" && d.Estado == "SP")), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
            Assert.IsTrue(controller.TempData.ContainsKey("SuccessMessage"));
        }

        [TestMethod]
        public void Create_Post_ModelInvalido_RetornaView()
        {
            // Arrange
            var cidadeModel = new CidadeModel { Nome = "", Estado = "SP" };
            var controller = CreateController();
            controller.ModelState.AddModelError("Nome", "Nome é obrigatório");

            // Act
            var result = controller.Create(cidadeModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(cidadeModel, viewResult.Model);
        }

        [TestMethod]
        public void Create_Post_ServiceException_AdicionaErroAoModelState()
        {
            // Arrange
            var cidadeModel = new CidadeModel { Nome = "São Paulo", Estado = "SP" };
            var cidadeDto = new CidadeDTO { Nome = "São Paulo", Estado = "SP" };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Create(It.IsAny<CidadeDTO>()))
                .Throws(new ServiceException("Cidade já cadastrada"));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CidadeDTO>(cidadeModel)).Returns(cidadeDto);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Act
            var result = controller.Create(cidadeModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(cidadeModel, viewResult.Model);
            Assert.IsTrue(controller.ModelState.ErrorCount > 0);
        }

        [TestMethod]
        public void Edit_Get_IdNulo_RetornaNotFound()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = controller.Edit(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Edit_Get_IdExistente_RetornaViewComModeloMapeado()
        {
            // Arrange
            var cidadeDto = new CidadeDTO { Id = 1, Nome = "São Paulo", Estado = "SP" };
            var cidadeModel = new CidadeModel { Id = 1, Nome = "São Paulo", Estado = "SP" };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Get(1)).Returns(cidadeDto);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CidadeModel>(cidadeDto)).Returns(cidadeModel);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(cidadeModel, viewResult.Model);
        }

        [TestMethod]
        public void Edit_Post_IdsDiferentes_RetornaNotFound()
        {
            // Arrange
            var cidadeModel = new CidadeModel { Id = 1, Nome = "São Paulo", Estado = "SP" };
            var controller = CreateController();

            // Act
            var result = controller.Edit(2, cidadeModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Edit_Post_ModelValido_ChamaUpdateERedireciona()
        {
            // Arrange
            var cidadeModel = new CidadeModel { Id = 1, Nome = "São Paulo Atualizado", Estado = "SP" };
            var cidadeDto = new CidadeDTO { Id = 1, Nome = "São Paulo Atualizado", Estado = "SP" };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Update(It.IsAny<CidadeDTO>())).Returns(true);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CidadeDTO>(cidadeModel)).Returns(cidadeDto);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Configurar TempData
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            // Act
            var result = controller.Edit(1, cidadeModel);

            // Assert
            cidadeServiceMock.Verify(s => s.Update(It.Is<CidadeDTO>(d => d.Id == 1 && d.Nome == "São Paulo Atualizado")), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
            Assert.IsTrue(controller.TempData.ContainsKey("SuccessMessage"));
        }

        [TestMethod]
        public void Edit_Post_UpdateRetornaFalse_RetornaNotFound()
        {
            // Arrange
            var cidadeModel = new CidadeModel { Id = 1, Nome = "São Paulo", Estado = "SP" };
            var cidadeDto = new CidadeDTO { Id = 1, Nome = "São Paulo", Estado = "SP" };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Update(It.IsAny<CidadeDTO>())).Returns(false);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CidadeDTO>(cidadeModel)).Returns(cidadeDto);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Act
            var result = controller.Edit(1, cidadeModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Delete_Get_IdNulo_RetornaNotFound()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = controller.Delete(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Delete_Get_IdExistente_RetornaViewComModeloMapeado()
        {
            // Arrange
            var cidadeDto = new CidadeDTO { Id = 1, Nome = "São Paulo", Estado = "SP" };
            var cidadeModel = new CidadeModel { Id = 1, Nome = "São Paulo", Estado = "SP" };

            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Get(1)).Returns(cidadeDto);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<CidadeModel>(cidadeDto)).Returns(cidadeModel);

            var controller = CreateController(cidadeServiceMock, mapperMock);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreSame(cidadeModel, viewResult.Model);
        }

        [TestMethod]
        public void DeleteConfirmed_ChamaDeleteERedireciona()
        {
            // Arrange
            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Delete(1)).Returns(true);

            var controller = CreateController(cidadeServiceMock);

            // Configurar TempData
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            // Act
            var result = controller.DeleteConfirmed(1);

            // Assert
            cidadeServiceMock.Verify(s => s.Delete(1), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
            Assert.IsTrue(controller.TempData.ContainsKey("SuccessMessage"));
        }

        [TestMethod]
        public void DeleteConfirmed_DeleteRetornaFalse_RedirecionaComMensagemErro()
        {
            // Arrange
            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Delete(1)).Returns(false);

            var controller = CreateController(cidadeServiceMock);

            // Configurar TempData
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            // Act
            var result = controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
            Assert.IsTrue(controller.TempData.ContainsKey("ErrorMessage"));
        }

        [TestMethod]
        public void DeleteConfirmed_ServiceException_RedirecionaComMensagemErro()
        {
            // Arrange
            var cidadeServiceMock = new Mock<ICidadeService>();
            cidadeServiceMock.Setup(s => s.Delete(1))
                .Throws(new ServiceException("Não é possível excluir a cidade"));

            var controller = CreateController(cidadeServiceMock);

            // Configurar TempData
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            // Act
            var result = controller.DeleteConfirmed(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
            Assert.IsTrue(controller.TempData.ContainsKey("ErrorMessage"));
        }
    }
}