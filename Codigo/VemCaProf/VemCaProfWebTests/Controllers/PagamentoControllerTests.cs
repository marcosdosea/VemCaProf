using System.Collections.Generic;
using System.Linq;
using Core.DTO;
using Core.Enums;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;

namespace VemCaProfWeb.Controllers.Tests
{
    [TestClass]
    public class PagamentoControllerTests
    {
        private static PagamentoController controller = null!;
        private Mock<IPagamentoService> mockService = null!;

        [TestInitialize]
        public void Initialize()
        {
            mockService = new Mock<IPagamentoService>();

            mockService.Setup(s => s.ListarPagamentos())
                .Returns(GetTestAulas());

            mockService.Setup(s => s.BuscarPorAula(It.IsAny<int>()))
                .Returns((int id) => GetTestAulas().FirstOrDefault(a => a.Id == id));

            mockService.Setup(s => s.RealizarPagamento(It.IsAny<RealizarPagamentoDTO>()))
                .Returns(true);

            controller = new PagamentoController(mockService.Object);
        }

        [TestMethod]
        public void Index_DeveRetornarViewComLista()
        {
            var result = controller.Index();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var view = (ViewResult)result;

            Assert.IsNotNull(view.Model);
            Assert.IsInstanceOfType(view.Model, typeof(IEnumerable<AulaDTO>));

            var lista = (IEnumerable<AulaDTO>)view.Model;
            Assert.AreEqual(2, lista.Count());
        }

        [TestMethod]
        public void Details_Valido_DeveRetornarView()
        {
            var result = controller.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var view = (ViewResult)result;

            Assert.IsInstanceOfType(view.Model, typeof(AulaDTO));
            var model = (AulaDTO)view.Model!;
            Assert.AreEqual(1, model.Id);
        }

        [TestMethod]
        public void Details_Invalido_DeveRetornarNotFound()
        {
            var result = controller.Details(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Create_Get_Valido_DeveRetornarViewComModel()
        {
            var result = controller.Create(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var view = (ViewResult)result;

            Assert.IsInstanceOfType(view.Model, typeof(PagamentoModel));
            var model = (PagamentoModel)view.Model!;
            Assert.AreEqual(1, model.IdAula);
            Assert.AreEqual(120.0, model.Valor);
        }

        [TestMethod]
        public void Create_Get_Invalido_DeveRetornarNotFound()
        {
            var result = controller.Create(999);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void Create_Post_Valido_DeveRedirecionarParaDetails()
        {
            var model = new PagamentoModel
            {
                IdAula = 1,
                Valor = 120.0,
                MetodoPagamento = MetodoPagamentoEnum.Pix,
                DescricaoAula = "Aula Pendente"
            };

            var result = controller.Create(model);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirect = (RedirectToActionResult)result;

            Assert.AreEqual("Details", redirect.ActionName);
            Assert.IsNotNull(redirect.RouteValues);
            Assert.AreEqual(1, redirect.RouteValues["id"]);
        }

        [TestMethod]
        public void Create_Post_ServiceException_DeveRetornarViewComErro()
        {
            mockService.Setup(s => s.RealizarPagamento(It.IsAny<RealizarPagamentoDTO>()))
                .Throws(new ServiceException("Falha ao pagar"));

            var model = new PagamentoModel
            {
                IdAula = 1,
                Valor = 120.0,
                MetodoPagamento = MetodoPagamentoEnum.Pix,
                DescricaoAula = "Aula Pendente"
            };

            var result = controller.Create(model);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsTrue(controller.ModelState.ErrorCount > 0);
        }

        private IEnumerable<AulaDTO> GetTestAulas()
        {
            return new List<AulaDTO>
            {
                new AulaDTO
                {
                    Id = 1,
                    Descricao = "Aula Pendente",
                    Valor = 120.0,
                    Status = StatusEnum.AguardandoPagamento,
                    MetodoPagamento = MetodoPagamentoEnum.Pix,
                    DataHoraPagamento = new System.DateTime(2000, 01, 01),
                    IdDisciplina = 1,
                    IdResponsavel = 1,
                    IdAluno = 2,
                    IdProfessor = 3,
                    DataHorarioInicio = new System.DateTime(2026, 02, 10, 14, 0, 0),
                    DataHorarioFinal = new System.DateTime(2026, 02, 10, 15, 0, 0),
                },
                new AulaDTO
                {
                    Id = 2,
                    Descricao = "Aula Paga",
                    Valor = 100.0,
                    Status = StatusEnum.Paga,
                    MetodoPagamento = MetodoPagamentoEnum.Credito,
                    DataHoraPagamento = new System.DateTime(2026, 02, 05, 09, 00, 00),
                    IdDisciplina = 1,
                    IdResponsavel = 1,
                    IdAluno = 2,
                    IdProfessor = 3,
                    DataHorarioInicio = new System.DateTime(2026, 02, 11, 14, 0, 0),
                    DataHorarioFinal = new System.DateTime(2026, 02, 11, 15, 0, 0),
                }
            };
        }
    }
}
