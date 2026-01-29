using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Mappers;
using VemCaProfWeb.Models;

namespace VemCaProfWebTests.Controllers
{
    [TestClass]
    public class ResponsavelPessoaControllerTests
    {
        private static ResponsavelPessoaController controller = null!;
        private Mock<IPessoaService> mockService = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            mockService = new Mock<IPessoaService>();

            // Configuração Real do AutoMapper
            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PessoaProfile())).CreateMapper();

            // Setup: Buscar todos os responsáveis
            mockService.Setup(service => service.GetAllResponsaveis())
                .Returns(GetTestResponsaveis());

            // Setup: Buscar responsável específico (ID 1)
            mockService.Setup(service => service.GetResponsavel(1))
                .Returns(GetTargetResponsavelEntity());

            // Setup: Métodos Void (Create, Edit, Delete)
            mockService.Setup(service => service.CreateResponsavel(It.IsAny<ResponsavelPessoaDTO>()))
                .Verifiable();
            mockService.Setup(service => service.EditResponsavel(It.IsAny<ResponsavelPessoaDTO>()))
                .Verifiable();
            mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            controller = new ResponsavelPessoaController(mockService.Object, mapper);
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<ResponsavelPessoaModel>));
            var lista = viewResult.ViewData.Model as List<ResponsavelPessoaModel>;
            
            Assert.IsNotNull(lista);
            Assert.AreEqual(2, lista.Count);
            Assert.AreEqual("Pai Responsável", lista[0].Nome);
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ResponsavelPessoaModel));
            var model = viewResult.ViewData.Model as ResponsavelPessoaModel;
            Assert.IsNotNull(model);

            Assert.AreEqual("Pai Responsável", model.Nome);
            Assert.AreEqual("Silva", model.Sobrenome);
            Assert.AreEqual("pai@email.com", model.Email);
            Assert.AreEqual("São Paulo", model.Cidade); 
            Assert.AreEqual(2, model.QuantidadeDeDependentes);
        }

        [TestMethod]
        public void CreateTest_Get_Valido()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void CreateTest_Post_Valid()
        {
            // Act
            var result = controller.Create(GetNewResponsavelModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);

            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);

            mockService.Verify(x => x.CreateResponsavel(It.IsAny<ResponsavelPessoaDTO>()), Times.Once);
        }

        [TestMethod]
        public void CreateTest_Post_Invalid()
        {
            // Arrange
            controller.ModelState.AddModelError("Nome", "Campo obrigatório");

            // Act
            var result = controller.Create(GetNewResponsavelModel());

            // Assert
            Assert.AreEqual(1, controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ResponsavelPessoaModel));
        }

        [TestMethod]
        public void EditTest_Get_Valid()
        {
            // Act
            var result = controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ResponsavelPessoaModel));
            var model = viewResult.ViewData.Model as ResponsavelPessoaModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Pai Responsável", model.Nome);
        }

        [TestMethod]
        public void EditTest_Post_Valid()
        {
            // Act
            var model = GetTargetResponsavelModel();
            var result = controller.Edit(model.Id, model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);

            Assert.AreEqual("Index", redirectToActionResult.ActionName);

            mockService.Verify(x => x.EditResponsavel(It.IsAny<ResponsavelPessoaDTO>()), Times.Once);
        }

        [TestMethod]
        public void EditTest_Post_IdMismatch()
        {
            // Act
            var model = GetTargetResponsavelModel();
            var result = controller.Edit(999, model); 

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            mockService.Verify(x => x.EditResponsavel(It.IsAny<ResponsavelPessoaDTO>()), Times.Never);
        }

        [TestMethod]
        public void DeleteTest_Get_Valid()
        {
            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.ViewData.Model as ResponsavelPessoaModel;
            Assert.IsNotNull(model);

            Assert.AreEqual("Pai Responsável", model.Nome);
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
           
            var result = controller.Delete(1, null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);

            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            mockService.Verify(x => x.Delete(1), Times.Once);
        }

        // --- MÉTODOS AUXILIARES COM DADOS COMPLETOS ---

        private ResponsavelPessoaModel GetNewResponsavelModel()
        {
            return new ResponsavelPessoaModel
            {
                Id = 3,
                Nome = "Novo Responsável",
                Sobrenome = "Teste",
                Cpf = "11122233344",
                Email = "novo@resp.com",
                Senha = "123",
                QuantidadeDeDependentes = 1,

                Telefone = "11999999999",
                Genero = "Feminino",
                DataNascimento = new DateTime(1985, 10, 10),
                Cep = "01000000",
                Rua = "Rua Nova",
                Numero = "50",
                Complemento = "Casa",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private ResponsavelPessoaModel GetTargetResponsavelModel()
        {
            return new ResponsavelPessoaModel
            {
                Id = 1,
                Nome = "Pai Responsável",
                Sobrenome = "Silva",
                Email = "pai@email.com",
                Senha = "", 
                QuantidadeDeDependentes = 2,

                // Dados Obrigatórios (PessoaDTO)
                Telefone = "11988888888",
                Genero = "Masculino",
                DataNascimento = new DateTime(1980, 1, 1),
                Cep = "02000000",
                Rua = "Rua Antiga",
                Numero = "100",
                Complemento = "Apto 10",
                Bairro = "Bairro Velho",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private static Pessoa GetTargetResponsavelEntity()
        {
            return new Pessoa
            {
                Id = 1,
                Nome = "Pai Responsável",
                Sobrenome = "Silva",
                Email = "pai@email.com",
                Cpf = "99988877766",
                QuantidadeDeDependentes = 2,

                Telefone = "11988888888",
                Genero = "Masculino",
                DataNascimento = new DateTime(1980, 1, 1),
                Cep = "02000000",
                Rua = "Rua Antiga",
                Numero = "100",
                Complemento = "Apto 10",
                Bairro = "Bairro Velho",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private IEnumerable<Pessoa> GetTestResponsaveis()
        {
            return new List<Pessoa>
            {
                new Pessoa
                {
                    Id = 1,
                    Nome = "Pai Responsável",
                    Sobrenome = "Silva",
                    QuantidadeDeDependentes = 2,
                    Cidade = "São Paulo",
                    Telefone = "11988888888"
                },
                new Pessoa
                {
                    Id = 2,
                    Nome = "Mãe Responsável",
                    Sobrenome = "Souza",
                    QuantidadeDeDependentes = 1,
                    Cidade = "Rio de Janeiro",
                    Telefone = "21999999999"
                }
            };
        }
    }
}