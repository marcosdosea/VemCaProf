using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Core;
using Core.Service;
using Core.DTO;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // para TempData

namespace VemCaProfWebTests
{
    [TestClass]
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

            // Configura TempData para evitar NullReferenceException
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Usuário autenticado padrão (Admin)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "12345678901"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Email, "usuario@teste.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // ==================== INDEX ====================

        [TestMethod]
        public void Index_DeveRetornarViewComListaDeModelos()
        {
            // Arrange
            var pessoas = new List<Pessoa> { new Pessoa { Id = 1, Nome = "João" } };
            var modelos = new List<PessoaModel> { new PessoaModel { Id = 1, Nome = "João" } };

            _pessoaServiceMock.Setup(s => s.GetListaParaIndex("P", "12345678901", true))
                .Returns(pessoas);
            _mapperMock.Setup(m => m.Map<List<PessoaModel>>(pessoas)).Returns(modelos);

            // Act
            var result = _controller.Index("P") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modelos, result.Model);
            Assert.AreEqual("P", result.ViewData["TipoAtual"]);
        }

        // ==================== DETAILS ====================

        [TestMethod]
        public void Details_ComPessoaExistente_RetornaViewComModelo()
        {
            // Arrange
            var pessoa = new Pessoa { Id = 1, Nome = "João", TipoPessoa = "P" };
            var modelo = new PessoaModel { Id = 1, Nome = "João" };

            _pessoaServiceMock.Setup(s => s.GetParaDetails(1, "12345678901", true))
                .Returns(pessoa);
            _mapperMock.Setup(m => m.Map<PessoaModel>(pessoa)).Returns(modelo);

            // Act
            var result = _controller.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modelo, result.Model);
        }

        [TestMethod]
        public void Details_PessoaNaoEncontrada_RedirecionaParaHomeIndex()
        {
            // Arrange
            _pessoaServiceMock.Setup(s => s.GetParaDetails(99, "12345678901", true))
                .Returns((Pessoa?)null);

            // Act
            var result = _controller.Details(99) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        // ==================== CREATE (GET) ====================

        [TestMethod]
        public void Create_Get_ComPessoaExistente_RedirecionaParaEdit()
        {
            // Arrange
            var pessoaExistente = new Pessoa { Id = 5, Cpf = "12345678901" };

            // Admin: isAdmin=true, isProfessor=false, isResponsavel=false
            _pessoaServiceMock.Setup(s => s.GetModelParaCreate("P", "12345678901", "usuario@teste.com", true, false, false))
                .Returns(pessoaExistente);

            // Act
            var result = _controller.Create("P") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(5, result.RouteValues!["id"]);
        }

        [TestMethod]
        public void Create_Get_ComNovaPessoa_RetornaViewComModelo()
        {
            // Arrange
            var novaEntidade = new Pessoa { TipoPessoa = "P", Cpf = "12345678901", Email = "usuario@teste.com" };
            var modelo = new PessoaModel { TipoPessoa = "P", Cpf = "12345678901", Email = "usuario@teste.com" };

            _pessoaServiceMock.Setup(s => s.GetModelParaCreate("P", "12345678901", "usuario@teste.com", true, false, false))
                .Returns(novaEntidade);
            _mapperMock.Setup(m => m.Map<PessoaModel>(novaEntidade)).Returns(modelo);

            // Configura serviços auxiliares
            _cidadeServiceMock.Setup(s => s.GetAll()).Returns(new List<CidadeDTO>());
            _disciplinaServiceMock.Setup(s => s.GetAll()).Returns(new List<Disciplina>());
            _pessoaServiceMock.Setup(s => s.GetAllResponsaveis()).Returns(new List<Pessoa>());

            // Act
            var result = _controller.Create("P") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var modelRetornado = result.Model as PessoaModel;
            Assert.IsNotNull(modelRetornado);
            Assert.AreEqual(modelo.TipoPessoa, modelRetornado.TipoPessoa);
            Assert.AreEqual(modelo.Cpf, modelRetornado.Cpf);
            Assert.AreEqual(modelo.Email, modelRetornado.Email);
        }

        // ==================== CREATE (POST) ====================

        [TestMethod]
        public void Create_Post_ModeloValido_RedirecionaParaDetails()
        {
            // Arrange
            var modelo = new PessoaModel { Id = 0, Nome = "Maria", Cpf = "12345678901", TipoPessoa = "A" };
            var entidade = new Pessoa { Id = 1, Nome = "Maria", Cpf = "12345678901" };

            _mapperMock.Setup(m => m.Map<Pessoa>(modelo)).Returns(entidade);

            // Act
            var result = _controller.Create(modelo) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(entidade.Id, result.RouteValues!["id"]);
            _pessoaServiceMock.Verify(s => s.CreateSeguro(entidade, "12345678901", true, false, false), Times.Once);
        }

        [TestMethod]
        public void Create_Post_ModeloInvalido_RetornaViewComModelo()
        {
            // Arrange
            _controller.ModelState.AddModelError("Nome", "Obrigatório");
            var modelo = new PessoaModel { TipoPessoa = "P" };

            // Act
            var result = _controller.Create(modelo) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modelo, result.Model);
            _pessoaServiceMock.Verify(s => s.CreateSeguro(It.IsAny<Pessoa>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        // ==================== EDIT (GET) ====================

        [TestMethod]
        public void Edit_Get_ComPessoaExistente_RetornaViewComModelo()
        {
            // Arrange
            var pessoa = new Pessoa { Id = 1, Nome = "João", TipoPessoa = "P" };
            var modelo = new PessoaModel { Id = 1, Nome = "João" };

            _pessoaServiceMock.Setup(s => s.GetParaEdit(1, "12345678901", true))
                .Returns(pessoa);
            _mapperMock.Setup(m => m.Map<PessoaModel>(pessoa)).Returns(modelo);

            _cidadeServiceMock.Setup(s => s.GetAll()).Returns(new List<CidadeDTO>());
            _disciplinaServiceMock.Setup(s => s.GetAll()).Returns(new List<Disciplina>());
            _pessoaServiceMock.Setup(s => s.GetAllResponsaveis()).Returns(new List<Pessoa>());

            // Act
            var result = _controller.Edit(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modelo, result.Model);
        }

        [TestMethod]
        public void Edit_Get_PessoaNaoEncontrada_RedirecionaParaHomeIndex()
        {
            // Arrange
            _pessoaServiceMock.Setup(s => s.GetParaEdit(99, "12345678901", true))
                .Returns((Pessoa?)null);

            // Act
            var result = _controller.Edit(99) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        // ==================== EDIT (POST) ====================

        [TestMethod]
        public void Edit_Post_ModeloValido_RedirecionaParaDetails()
        {
            // Arrange
            var modelo = new PessoaModel { Id = 1, Nome = "João Editado", Cpf = "12345678901" };
            var entidade = new Pessoa { Id = 1, Nome = "João Editado" };

            _mapperMock.Setup(m => m.Map<Pessoa>(modelo)).Returns(entidade);
            _pessoaServiceMock.Setup(s => s.EditSeguro(entidade, "12345678901", true))
                .Returns(true);

            // Act
            var result = _controller.Edit(1, modelo) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(1, result.RouteValues!["id"]);
            _pessoaServiceMock.Verify(s => s.EditSeguro(entidade, "12345678901", true), Times.Once);
        }

        [TestMethod]
        public void Edit_Post_ModeloInvalido_RetornaView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Nome", "Obrigatório");
            var modelo = new PessoaModel { Id = 1, TipoPessoa = "P" };

            // Act
            var result = _controller.Edit(1, modelo) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modelo, result.Model);
            _pessoaServiceMock.Verify(s => s.EditSeguro(It.IsAny<Pessoa>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void Edit_Post_SemPermissao_AdicionaErroERetornaView()
        {
            // Arrange
            var modelo = new PessoaModel { Id = 1, Nome = "João", Cpf = "12345678901", TipoPessoa = "P" };
            var entidade = new Pessoa { Id = 1, Nome = "João" };

            _mapperMock.Setup(m => m.Map<Pessoa>(modelo)).Returns(entidade);
            _pessoaServiceMock.Setup(s => s.EditSeguro(entidade, "12345678901", true))
                .Returns(false);

            // Act
            var result = _controller.Edit(1, modelo) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modelo, result.Model);
            Assert.IsFalse(_controller.ModelState.IsValid);
            _pessoaServiceMock.Verify(s => s.EditSeguro(entidade, "12345678901", true), Times.Once);
        }

        [TestMethod]
        public void Edit_Post_IdDiferente_RetornaNotFound()
        {
            // Arrange
            var modelo = new PessoaModel { Id = 2 };

            // Act
            var result = _controller.Edit(1, modelo);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        // ==================== DELETE (GET) ====================

        [TestMethod]
        public void Delete_Get_ComPessoaExistente_RetornaView()
        {
            // Arrange
            var pessoa = new Pessoa { Id = 1, Nome = "João" };
            var modelo = new PessoaModel { Id = 1, Nome = "João" };

            _pessoaServiceMock.Setup(s => s.GetParaDelete(1, "12345678901", true))
                .Returns(pessoa);
            _mapperMock.Setup(m => m.Map<PessoaModel>(pessoa)).Returns(modelo);

            // Act
            var result = _controller.Delete(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(modelo, result.Model);
        }

        [TestMethod]
        public void Delete_Get_PessoaNaoEncontrada_RedirecionaParaHomeIndex()
        {
            // Arrange
            _pessoaServiceMock.Setup(s => s.GetParaDelete(99, "12345678901", true))
                .Returns((Pessoa?)null);

            // Act
            var result = _controller.Delete(99) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        // ==================== DELETE CONFIRMED ====================

        [TestMethod]
        public void DeleteConfirmed_ComSucesso_RedirecionaParaIndex()
        {
            // Arrange
            _pessoaServiceMock.Setup(s => s.DeleteSeguro(1, "12345678901", true))
                .Returns(true);

            // Act
            var result = _controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            _pessoaServiceMock.Verify(s => s.DeleteSeguro(1, "12345678901", true), Times.Once);
        }

        [TestMethod]
        public void DeleteConfirmed_Falha_RedirecionaParaIndexComErro()
        {
            // Arrange
            _pessoaServiceMock.Setup(s => s.DeleteSeguro(1, "12345678901", true))
                .Returns(false);

            // Act
            var result = _controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsTrue(_controller.TempData.ContainsKey("Error"));
            _pessoaServiceMock.Verify(s => s.DeleteSeguro(1, "12345678901", true), Times.Once);
        }

        // ==================== MEU PERFIL ====================

        [TestMethod]
        public void MeuPerfil_ComPessoaExistente_RedirecionaParaDetails()
        {
            // Arrange
            var pessoa = new Pessoa { Id = 5, Cpf = "12345678901" };
            _pessoaServiceMock.Setup(s => s.GetByCpf("12345678901")).Returns(pessoa);

            // Act
            var result = _controller.MeuPerfil() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(5, result.RouteValues!["id"]);
        }

        [TestMethod]
        public void MeuPerfil_SemPessoa_RedirecionaParaCreateComTipoBaseadoNaRole()
        {
            // Altera o usuário para ter apenas a role "Professor"
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "12345678901"),
                new Claim(ClaimTypes.Role, "Professor")
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = user;

            _pessoaServiceMock.Setup(s => s.GetByCpf("12345678901")).Returns((Pessoa?)null);

            // Act
            var result = _controller.MeuPerfil() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Create", result.ActionName);
            Assert.AreEqual("P", result.RouteValues!["tipo"]);
        }
    }
}