using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Core;
using Core.Service;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace VemCaProfWebTests
{
    [TestClass()]
    public class PessoaControllerTests
    {
        private Mock<IPessoaService> _pessoaServiceMock = null!;
        private Mock<ICidadeService> _cidadeServiceMock = null!;
        private Mock<IDisciplinaService> _disciplinaServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private PessoaController _controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            _pessoaServiceMock = new Mock<IPessoaService>();
            _cidadeServiceMock = new Mock<ICidadeService>();
            _disciplinaServiceMock = new Mock<IDisciplinaService>();
            _mapperMock = new Mock<IMapper>();

            _controller = new PessoaController(
                _pessoaServiceMock.Object,
                _cidadeServiceMock.Object,
                _disciplinaServiceMock.Object,
                _mapperMock.Object);

            // Mockando o usuário logado (necessário por causa do [Authorize] e do User.Identity.Name)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "11111111111"), // CPF fictício
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, "teste@teste.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [TestMethod()]
        public void Index_RetornaViewComListaDeModelos()
        {
            // Arrange
            var listaEntities = new List<Pessoa> { new Pessoa { Id = 1, Nome = "João", TipoPessoa = "P" } };
            var listaModels = new List<PessoaModel> { new PessoaModel { Id = 1, Nome = "João" } };

            _pessoaServiceMock.Setup(s => s.GetAll()).Returns(listaEntities);
            _mapperMock.Setup(m => m.Map<List<PessoaModel>>(It.IsAny<IEnumerable<Pessoa>>())).Returns(listaModels);

            // Act
            var result = _controller.Index("P") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(listaModels, result.Model);
        }

        [TestMethod()]
        public void Details_RetornaViewSeIdExistir()
        {
            // Arrange
            var entity = new Pessoa { Id = 1, Nome = "João" };
            var model = new PessoaModel { Id = 1, Nome = "João" };

            _pessoaServiceMock.Setup(s => s.Get(1)).Returns(entity);
            _mapperMock.Setup(m => m.Map<PessoaModel>(entity)).Returns(model);

            // Act
            var result = _controller.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod()]
        public void Create_Post_RedirecionaParaHomeAoCriarComSucesso()
        {
            // Arrange
            var model = new PessoaModel { Nome = "Maria", Cpf = "22222222222", TipoPessoa = "A" };
            var entity = new Pessoa { Nome = "Maria", Cpf = "22222222222" };

            _mapperMock.Setup(m => m.Map<Pessoa>(model)).Returns(entity);

            // Act
            var result = _controller.Create(model, null, null, null) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            _pessoaServiceMock.Verify(s => s.Create(It.IsAny<Pessoa>()), Times.Once());
        }

        [TestMethod()]
        public void Edit_Get_RetornaViewSeUsuarioForDonoOuAdmin()
        {
            // Arrange
            var entity = new Pessoa { Id = 1, Cpf = "11111111111", TipoPessoa = "P" };
            var model = new PessoaModel { Id = 1, Cpf = "11111111111" };

            _pessoaServiceMock.Setup(s => s.Get(1)).Returns(entity);
            _mapperMock.Setup(m => m.Map<PessoaModel>(entity)).Returns(model);
            _disciplinaServiceMock.Setup(s => s.GetAll()).Returns(new List<Disciplina>());

            // Act
            var result = _controller.Edit(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod()]
        public void Delete_Post_ChamaDeleteESalva()
        {
            // Arrange
            var entity = new Pessoa { Id = 1, Cpf = "11111111111" };
            _pessoaServiceMock.Setup(s => s.Get(1)).Returns(entity);

            // Act
            var result = _controller.Delete(1, new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>())) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            _pessoaServiceMock.Verify(s => s.Delete(1), Times.Once());
        }

        [TestMethod()]
        public void Create_Get_SePessoaJaExiste_RedirecionaParaEdit()
        {
            // Arrange
            var pessoaExistente = new Pessoa { Id = 5, Cpf = "11111111111" };
            _pessoaServiceMock.Setup(s => s.GetByCpf("11111111111")).Returns(pessoaExistente);

            // Act
            var result = _controller.Create("P") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(5, result.RouteValues["id"]);
        }
    }
}