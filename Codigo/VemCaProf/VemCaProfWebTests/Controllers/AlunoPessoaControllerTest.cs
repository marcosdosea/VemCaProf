using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Mappers;
using VemCaProfWeb.Models;
using VemCaProfWeb.Areas.Identity.Data; 

namespace VemCaProfWebTests.Controllers
{
    [TestClass]
    public class AlunoPessoaControllerTests
    {
        private AlunoPessoaController _controller = null!;
        private Mock<IPessoaService> _mockService = null!;
        private Mock<UserManager<Usuario>> _mockUserManager = null!; 
        private readonly string _fakeGuid = Guid.NewGuid().ToString(); 

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            _mockService = new Mock<IPessoaService>();

            var store = new Mock<IUserStore<Usuario>>();
            _mockUserManager = new Mock<UserManager<Usuario>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PessoaProfile())).CreateMapper();

            _mockService.Setup(service => service.GetAllAlunos())
                .Returns(GetTestAlunos());

            _mockService.Setup(service => service.GetAllResponsaveis())
                .Returns(GetTestResponsaveis());

            _mockService.Setup(service => service.GetAluno(1))
                .Returns(GetTargetAlunoEntity());

            _mockService.Setup(service => service.CreateAluno(It.IsAny<AlunoPessoaDTO>()))
                .Verifiable();
            _mockService.Setup(service => service.EditAluno(It.IsAny<AlunoPessoaDTO>()))
                .Verifiable();
            _mockService.Setup(service => service.Delete(It.IsAny<int>()))
                .Verifiable();

            // Invocado com os 3 parâmetros agora exigidos (Service, Mapper, UserManager)
            _controller = new AlunoPessoaController(_mockService.Object, mapper, _mockUserManager.Object);
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<AlunoPessoaModel>));

            var lista = viewResult.ViewData.Model as List<AlunoPessoaModel>;
            Assert.IsNotNull(lista);
            Assert.AreEqual(2, lista.Count);
            
            Assert.AreEqual("Joãozinho", lista[0].Nome);
            Assert.IsTrue(lista[0].AlunoDeMenor);
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            // Act
            var result = _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(AlunoPessoaModel));
            var alunoModel = viewResult.ViewData.Model as AlunoPessoaModel;
            Assert.IsNotNull(alunoModel);

            Assert.AreEqual("Joãozinho", alunoModel.Nome);
            Assert.AreEqual("Silva", alunoModel.Sobrenome);
            Assert.AreEqual("joao@email.com", alunoModel.Email);
            Assert.AreEqual(10, alunoModel.IdResponsavel); 
            Assert.IsTrue(alunoModel.AlunoDeMenor);
            Assert.IsFalse(alunoModel.Atipico);
            
            Assert.AreEqual("São Paulo", alunoModel.Cidade);
        }

        [TestMethod]
        public void CreateTest_Get_Valido()
        {
            // Act - Alterado para passar IdUsuario e email como o Identity exige
            var result = _controller.Create(_fakeGuid, "teste@email.com");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            // Verifica se o Model foi preenchido com os dados do Identity
            var model = viewResult.Model as AlunoPessoaModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(_fakeGuid, model.IdUsuario);
            Assert.AreEqual("teste@email.com", model.Email);

            Assert.IsNotNull(viewResult.ViewData["ListaResponsaveis"]);
            Assert.IsInstanceOfType(viewResult.ViewData["ListaResponsaveis"], typeof(SelectList));
        }

        [TestMethod]
        public void CreateTest_Post_Valid()
        {
            // Act
            var result = _controller.Create(GetNewAlunoModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            
            _mockService.Verify(x => x.CreateAluno(It.IsAny<AlunoPessoaDTO>()), Times.Once);
        }

        [TestMethod]
        public void CreateTest_Post_Invalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Nome", "Campo requerido");

            // Act
            var result = _controller.Create(GetNewAlunoModel());

            // Assert
            Assert.AreEqual(1, _controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult)); 
            
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            
            Assert.IsNotNull(viewResult.ViewData["ListaResponsaveis"]);
        }
        

        [TestMethod]
        public void EditTest_Post_Valid()
        {
            // Act
            var model = GetTargetAlunoModel();
            var result = _controller.Edit(model.Id, model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            
            Assert.AreEqual("Index", redirectToActionResult.ActionName);

            _mockService.Verify(x => x.EditAluno(It.IsAny<AlunoPessoaDTO>()), Times.Once);
        }
        
        [TestMethod]
        public void EditTest_Post_IdMismatch()
        {
            // Act
            var model = GetTargetAlunoModel();
            var result = _controller.Edit(999, model); 

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockService.Verify(x => x.EditAluno(It.IsAny<AlunoPessoaDTO>()), Times.Never);
        }

        [TestMethod]
        public void DeleteTest_Get_Valid()
        {
            // Act
            var result = _controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            
            var model = viewResult.ViewData.Model as AlunoPessoaModel;
            Assert.IsNotNull(model);
            
            Assert.AreEqual("Joãozinho", model.Nome);
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
            var result = _controller.Delete(1, null!);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
    
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            _mockService.Verify(x => x.Delete(1), Times.Once);
        }

        // --- MÉTODOS AUXILIARES ATUALIZADOS COM CAMPOS DO PESSOA DTO ---

        private AlunoPessoaModel GetNewAlunoModel()
        {
            return new AlunoPessoaModel
            {
                Id = 3,
                IdUsuario = _fakeGuid, 
                Nome = "Novo Aluno",
                Sobrenome = "Teste",
                Cpf = "11122233344",
                Email = "novo@aluno.com",
                AlunoDeMenor = true,
                Atipico = false,
                IdResponsavel = 10,

                // Campos Obrigatórios de PessoaDTO
                Telefone = "11999999999",
                Genero = "Masculino",
                DataNascimento = new DateTime(2015, 5, 5),
                Cep = "01000000",
                Rua = "Rua da Escola",
                Numero = "10",
                Complemento = "",
                Bairro = "Centro",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private Pessoa GetTargetAlunoEntity()
        {
            return new Pessoa
            {
                Id = 1,
                IdUsuario = _fakeGuid,
                Nome = "Joãozinho",
                Sobrenome = "Silva",
                Email = "joao@email.com",
                Cpf = "12345678900",
                AlunoDeMenor = true,
                Atipico = false,
                ResponsavelId = 10, 
                IdCidade = null,

                // Campos Obrigatórios de PessoaDTO/Entity
                Telefone = "11988888888",
                Genero = "Masculino",
                DataNascimento = new DateTime(2014, 1, 1),
                Cep = "02000000",
                Rua = "Rua de Casa",
                Numero = "20",
                Complemento = "Fundos",
                Bairro = "Bairro Novo",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private AlunoPessoaModel GetTargetAlunoModel()
        {
            return new AlunoPessoaModel
            {
                Id = 1,
                IdUsuario = _fakeGuid, 
                Nome = "Joãozinho",
                Sobrenome = "Silva",
                Email = "joao@email.com",
                // Senha removida
                IdResponsavel = 10,
                AlunoDeMenor = true,
                Atipico = false,

                // Campos Obrigatórios de PessoaDTO
                Telefone = "11988888888",
                Genero = "Masculino",
                DataNascimento = new DateTime(2014, 1, 1),
                Cep = "02000000",
                Rua = "Rua de Casa",
                Numero = "20",
                Complemento = "Fundos",
                Bairro = "Bairro Novo",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private IEnumerable<Pessoa> GetTestAlunos()
        {
            return new List<Pessoa>
            {
                new Pessoa
                {
                    Id = 1,
                    IdUsuario = Guid.NewGuid().ToString(),
                    Nome = "Joãozinho",
                    Sobrenome = "Silva",
                    AlunoDeMenor = true,
                    Atipico = false,
                    Cidade = "São Paulo",
                    Telefone = "11988888888"
                },
                new Pessoa
                {
                    Id = 2,
                    IdUsuario = Guid.NewGuid().ToString(),
                    Nome = "Mariazinha",
                    Sobrenome = "Souza",
                    AlunoDeMenor = false,
                    Atipico = true,
                    Cidade = "Rio de Janeiro",
                    Telefone = "21999999999"
                }
            };
        }

        private IEnumerable<Pessoa> GetTestResponsaveis()
        {
            return new List<Pessoa>
            {
                new Pessoa
                {
                    Id = 10,
                    Nome = "Pai Responsável",
                    Sobrenome = "Silva",
                    Cpf = "99988877766"
                },
                new Pessoa
                {
                    Id = 11,
                    Nome = "Mãe Responsável",
                    Sobrenome = "Souza",
                    Cpf = "55544433322"
                }
            };
        }
    }
}