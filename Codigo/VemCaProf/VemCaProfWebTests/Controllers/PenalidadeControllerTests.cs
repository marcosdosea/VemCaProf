using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;
using System.Collections.Generic;
using System.Linq;

namespace VemCaProfWebTests.Controllers
{
    [TestClass]
    public class PenalidadeControllerTests
    {
        private PenalidadeController controller = null!;

        private Mock<IPenalidadeService> mockPenalidadeService = null!;
        private Mock<IPessoaService> mockPessoaService = null!;
        private Mock<IMapper> mockMapper = null!;
        private Mock<ILogger<PenalidadeController>> mockLogger = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockPenalidadeService = new Mock<IPenalidadeService>();
            mockPessoaService = new Mock<IPessoaService>();
            mockMapper = new Mock<IMapper>();
            mockLogger = new Mock<ILogger<PenalidadeController>>();

            controller = new PenalidadeController(
                mockPenalidadeService.Object,
                mockPessoaService.Object,
                mockMapper.Object,
                mockLogger.Object
            );
        }

        [TestMethod]
        public void Index_ReturnView()
        {
            // Arrange
            var penalidades = new List<PenalidadeDTO>
            {
                new PenalidadeDTO
                {
                    Id = 1,
                    Descricao = "Teste"
                }
            };

            var models = new List<PenalidadeModel>
            {
                new PenalidadeModel
                {
                    Id = 1,
                    Descricao = "Teste"
                }
            };

            mockPenalidadeService
                .Setup(s => s.GetAll())
                .Returns(penalidades);

            mockMapper
                .Setup(m => m.Map<IEnumerable<PenalidadeModel>>(penalidades))
                .Returns(models);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<PenalidadeModel>));
        }

        [TestMethod]
        public void Details_ReturnNotFound()
        {
            mockPenalidadeService
                .Setup(s => s.Get(1))
                .Returns((PenalidadeDTO)null!);

            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Details_ReturnView()
        {
            var dto = new PenalidadeDTO
            {
                Id = 1,
                Descricao = "Teste"
            };

            var model = new PenalidadeModel
            {
                Id = 1,
                Descricao = "Teste"
            };

            mockPenalidadeService
                .Setup(s => s.Get(1))
                .Returns(dto);

            mockMapper
                .Setup(m => m.Map<PenalidadeModel>(dto))
                .Returns(model);

            var result = controller.Details(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public void Create_Post_RedirectSuccess()
        {
            var model = new PenalidadeModel
            {
                Descricao = "Nova penalidade"
            };

            var dto = new PenalidadeDTO
            {
                Descricao = "Nova penalidade"
            };

            mockMapper
                .Setup(m => m.Map<PenalidadeDTO>(model))
                .Returns(dto);

            var result = controller.Create(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void Create_Post_ModelInvalid_ReturnView()
        {
            controller.ModelState.AddModelError("Erro", "Erro");

            var model = new PenalidadeModel();

            var result = controller.Create(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public void Edit_Get_NotFound()
        {
            mockPenalidadeService
                .Setup(s => s.Get(1))
                .Returns((PenalidadeDTO)null!);

            var result = controller.Edit(1);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Edit_Get_ReturnView()
        {
            var dto = new PenalidadeDTO
            {
                Id = 1,
                Descricao = "Teste"
            };

            var model = new PenalidadeModel
            {
                Id = 1,
                Descricao = "Teste"
            };

            mockPenalidadeService
                .Setup(s => s.Get(1))
                .Returns(dto);

            mockMapper
                .Setup(m => m.Map<PenalidadeModel>(dto))
                .Returns(model);

            var result = controller.Edit(1) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public void Edit_Post_RedirectSuccess()
        {
            var model = new PenalidadeModel
            {
                Id = 1,
                Descricao = "Atualizada"
            };

            var dto = new PenalidadeDTO
            {
                Id = 1,
                Descricao = "Atualizada"
            };

            mockMapper
                .Setup(m => m.Map<PenalidadeDTO>(model))
                .Returns(dto);

            var result = controller.Edit(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void Delete_Get_NotFound()
        {
            mockPenalidadeService
                .Setup(s => s.Get(1))
                .Returns((PenalidadeDTO)null!);

            var result = controller.Delete(1);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Delete_Post_Redirect()
        {
            var result = controller.Delete(1, new PenalidadeModel())
                as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
    }
}