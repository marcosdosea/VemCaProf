using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service;
using System;
using System.Collections.Generic;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Tests.Controllers
{
    [TestClass]
    public class DisponibilidadeHorarioControllerTests
    {
        private DisponibilidadeHorarioController CreateController(
            Mock<IDisponibilidadeHorarioService>? serviceMock = null,
            Mock<IPessoaService>? pessoaServiceMock = null,
            Mock<IMapper>? mapperMock = null,
            Mock<ILogger<DisponibilidadeHorarioController>>? loggerMock = null)
        {
            serviceMock ??= new Mock<IDisponibilidadeHorarioService>();
            pessoaServiceMock ??= new Mock<IPessoaService>();
            mapperMock ??= new Mock<IMapper>();
            loggerMock ??= new Mock<ILogger<DisponibilidadeHorarioController>>();

            return new DisponibilidadeHorarioController(
                serviceMock.Object,
                pessoaServiceMock.Object,
                mapperMock.Object,
                loggerMock.Object
            );
        }

        [TestMethod]
        public void Index_RetornaViewComListaMapeada()
        {
            var dia = DateTime.Today;

            var dto = new DisponibilidadeHorarioDTO { Id = 1, Dia = dia };
            var dtos = new List<DisponibilidadeHorarioDTO> { dto };

            var model = new DisponibilidadeHorarioModel { Id = 1, Dia = dia };
            var models = new List<DisponibilidadeHorarioModel> { model };

            var serviceMock = new Mock<IDisponibilidadeHorarioService>();
            serviceMock.Setup(s => s.GetAll()).Returns(dtos);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<DisponibilidadeHorarioModel>>(dtos))
                      .Returns(models);

            var controller = CreateController(serviceMock, mapperMock: mapperMock);

            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            CollectionAssert.AreEqual(models, ((IEnumerable<DisponibilidadeHorarioModel>)((ViewResult)result).Model!).ToList());
        }

        [TestMethod]
        public void Details_IdValido_RetornaViewComModeloMapeado()
        {
            var dia = DateTime.Today;

            var dto = new DisponibilidadeHorarioDTO { Id = 2, Dia = dia };
            var model = new DisponibilidadeHorarioModel { Id = 2, Dia = dia };

            var serviceMock = new Mock<IDisponibilidadeHorarioService>();
            serviceMock.Setup(s => s.Get(2)).Returns(dto);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<DisponibilidadeHorarioModel>(dto))
                      .Returns(model);

            var controller = CreateController(serviceMock, mapperMock: mapperMock);

            var result = controller.Details(2);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreSame(model, ((ViewResult)result).Model);
        }
        [TestMethod]
        public void Create_Post_ModelValido_CriaERedireciona()
        {

            // Arrange
            var dia = DateTime.Today;
            var disponibilidadeHorarioModel = new DisponibilidadeHorarioModel { Id = 1, Dia = dia };
            var disponibilidadeHorarioDto = new DisponibilidadeHorarioDTO { Id = 1, Dia = dia };

            var disponibilidadeHorarioServiceMock = new Mock<IDisponibilidadeHorarioService>();
            disponibilidadeHorarioServiceMock.Setup(s => s.Create(It.IsAny<DisponibilidadeHorarioDTO>())).Returns(1);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<DisponibilidadeHorarioDTO>(disponibilidadeHorarioModel)).Returns(disponibilidadeHorarioDto);

            var controller = CreateController(disponibilidadeHorarioServiceMock, mapperMock: mapperMock);

            // Configurar TempData
            controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());

            // Act
            var result = controller.Create(disponibilidadeHorarioModel);

            // Assert

            disponibilidadeHorarioServiceMock.Verify(s => s.Create(It.Is<DisponibilidadeHorarioDTO>(d => d.Id == 1 && d.Dia == dia)), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual(nameof(controller.Index), redirect.ActionName);
            Assert.IsTrue(controller.TempData.ContainsKey("SuccessMessage"));
        }

        [TestMethod]
        public void Create_Get_CarregaProfessoresSemExibirId()
        {
            var pessoaServiceMock = new Mock<IPessoaService>();
            pessoaServiceMock.Setup(s => s.GetAllProfessores()).Returns(new[]
            {
                new Pessoa
                {
                    Id = 10,
                    Nome = "Paulo",
                    Sobrenome = "Professor",
                    Email = "paulo@teste.com",
                    TipoPessoa = "P"
                }
            });
            var controller = CreateController(pessoaServiceMock: pessoaServiceMock);

            var result = controller.Create();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var opcao = ((SelectList)controller.ViewBag.Professores).Single();
            Assert.AreEqual("Paulo Professor - paulo@teste.com", opcao.Text);
            Assert.AreEqual("10", opcao.Value);
        }

        [TestMethod]
        public void Create_PostInvalido_RecarregaProfessores()
        {
            var pessoaServiceMock = new Mock<IPessoaService>();
            pessoaServiceMock.Setup(s => s.GetAllProfessores()).Returns(new[]
            {
                new Pessoa { Id = 10, Nome = "Paulo", Sobrenome = "Professor", Email = "paulo@teste.com" }
            });
            var controller = CreateController(pessoaServiceMock: pessoaServiceMock);
            controller.ModelState.AddModelError("Dia", "Dia inválido");

            var result = controller.Create(new DisponibilidadeHorarioModel { IdProfessor = 10 });

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Paulo Professor - paulo@teste.com", ((SelectList)controller.ViewBag.Professores).Single().Text);
        }

    }
}
