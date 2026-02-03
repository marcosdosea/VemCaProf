using AutoMapper;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            Mock<IMapper>? mapperMock = null,
            Mock<ILogger<DisponibilidadeHorarioController>>? loggerMock = null)
        {
            serviceMock ??= new Mock<IDisponibilidadeHorarioService>();
            mapperMock ??= new Mock<IMapper>();
            loggerMock ??= new Mock<ILogger<DisponibilidadeHorarioController>>();

            return new DisponibilidadeHorarioController(
                serviceMock.Object,
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

            var controller = CreateController(serviceMock, mapperMock);

            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreSame(models, ((ViewResult)result).Model);
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

            var controller = CreateController(serviceMock, mapperMock);

            var result = controller.Details(2);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreSame(model, ((ViewResult)result).Model);
        }

        [TestMethod]
        public void Create_Post_ModelInvalido_RetornaView()
        {
            var controller = CreateController();
            controller.ModelState.AddModelError("Dia", "Obrigatório");

            var result = controller.Create(new DisponibilidadeHorarioModel());

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
