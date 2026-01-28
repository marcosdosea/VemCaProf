using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Mappers;
using VemCaProfWeb.Models;

namespace VemCaProfWebTests.Controllers
{
    [TestClass]
    public class ProfessorPessoaControllerTests
    {
        private ProfessorPessoaController _controller = null!;
        private Mock<IPessoaService> _mockPessoaService = null!;
        private Mock<ICidadeService> _mockCidadeService = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            _mockPessoaService = new Mock<IPessoaService>();
            _mockCidadeService = new Mock<ICidadeService>();
            
            // Configuração Real do AutoMapper
            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PessoaProfile())).CreateMapper();

            // Setup: Buscar todos os responsáveis
            _mockPessoaService.Setup(s => s.GetAllProfessores())
                .Returns(GetTestProfessores());

            // Setup: Buscar responsável específico (ID 1)
            _mockCidadeService.Setup(s => s.GetAll())
                .Returns(GetTestCidades());

            _mockPessoaService.Setup(s => s.GetProfessor(1))
                .Returns(GetTargetProfessorEntity());

            // Setup: Métodos Void (Create, Edit, Delete)
            _mockPessoaService.Setup(s => s.CreateProfessor(It.IsAny<ProfessorPessoaDTO>()))
                .Verifiable();
            _mockPessoaService.Setup(s => s.EditProfessor(It.IsAny<ProfessorPessoaDTO>()))
                .Verifiable();
            _mockPessoaService.Setup(s => s.Delete(It.IsAny<int>()))
                .Verifiable();

            _controller = new ProfessorPessoaController(_mockPessoaService.Object, _mockCidadeService.Object, mapper);
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            // Act
            var result = _controller.Index();

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<ProfessorPessoaModel>));
            var lista = viewResult.ViewData.Model as List<ProfessorPessoaModel>;
            Assert.IsNotNull(lista);
            
            Assert.AreEqual(2, lista.Count);
            Assert.AreEqual("Professor Girafales", lista[0].Nome);
            Assert.IsTrue(lista[0].Libras);
        }

        [TestMethod]
        public void DetailsTest_Valido()
        {
            // Act
            var result = _controller.Details(1);
            
            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ProfessorPessoaModel));
            var model = viewResult.ViewData.Model as ProfessorPessoaModel;
            Assert.IsNotNull(model);

            Assert.AreEqual("Professor Girafales", model.Nome);
            Assert.AreEqual("Matemática", model.DescricaoProfessor);
            Assert.AreEqual(100, model.IdCidade);
            Assert.AreEqual("São Paulo", model.Cidade);
        }

        [TestMethod]
        public void CreateTest_Get_Valido()
        {
            // Act
            var result = _controller.Create();

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            Assert.IsNotNull(viewResult.ViewData["ListaDeCidades"]);
            Assert.IsInstanceOfType(viewResult.ViewData["ListaDeCidades"], typeof(SelectList));
        }

        [TestMethod]
        public void CreateTest_Post_Valid_SemArquivo()
        {
            // Arrange
            var model = GetNewProfessorModel();
            
            // Act
            var result = _controller.Create(model, null, null);

            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);

            _mockPessoaService.Verify(s => s.CreateProfessor(It.IsAny<ProfessorPessoaDTO>()), Times.Once);
        }

        [TestMethod]
        public void CreateTest_Post_Valid_ComArquivoPequeno()
        {
            // Arrange
            var model = GetNewProfessorModel();
            var mockFile = CreateMockFile("foto.jpg", 1024); 

            // Act
            var result = _controller.Create(model, null, mockFile);

            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            _mockPessoaService.Verify(s => s.CreateProfessor(It.IsAny<ProfessorPessoaDTO>()), Times.Once);
        }

        [TestMethod]
        public void CreateTest_Post_ArquivoMuitoGrande_LancaExcecao()
        {
            // Arrange
            var model = GetNewProfessorModel();
            var mockFile = CreateMockFile("grande.pdf", 70000); 

            // Act
            Action act = () => _controller.Create(model, mockFile, null);
            var ex = Assert.ThrowsException<InvalidOperationException>(act);

            //Assert
            Assert.AreEqual("O arquivo 'grande.pdf' excede o limite de 60KB.", ex.Message);
            _mockPessoaService.Verify(s => s.CreateProfessor(It.IsAny<ProfessorPessoaDTO>()), Times.Never);
        }

        [TestMethod]
        public void CreateTest_Post_Invalid_SemSenha()
        {
            // Arrange
            var model = GetNewProfessorModel();
            model.Senha = null!; 

            // Act
            var result = _controller.Create(model, null, null);
            
            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            Assert.IsTrue(_controller.ModelState.ContainsKey("Senha"));
            Assert.IsNotNull(viewResult.ViewData["ListaDeCidades"]);
        }

        [TestMethod]
        public void EditTest_Get_Valid()
        {
            // Act
            var result = _controller.Edit(1);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.ViewData.Model as ProfessorPessoaModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);

            Assert.IsNotNull(viewResult.ViewData["ListaDeCidades"]);
        }

        [TestMethod]
        public void EditTest_Post_Valid()
        {
            // Arrange
            var model = GetTargetProfessorModel();
            
            // Act
            var result = _controller.Edit(model.Id, model, null, null);
            
            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);

            _mockPessoaService.Verify(s => s.EditProfessor(It.IsAny<ProfessorPessoaDTO>()), Times.Once);
        }

        [TestMethod]
        public void EditTest_Post_IdMismatch()
        {
            // Arrange
            var model = GetTargetProfessorModel();
            
            // Act
            var result = _controller.Edit(999, model, null, null);
            
            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _mockPessoaService.Verify(s => s.EditProfessor(It.IsAny<ProfessorPessoaDTO>()), Times.Never);
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

            var model = viewResult.ViewData.Model as ProfessorPessoaModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("Professor Girafales", model.Nome);
        }

        [TestMethod]
        public void DeleteTest_Post_Valid()
        {
            // Act
            var result = _controller.Delete(1, null!);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);

            _mockPessoaService.Verify(s => s.Delete(1), Times.Once);
        }

        // --- MÉTODOS AUXILIARES ATUALIZADOS COM CAMPOS OBRIGATÓRIOS ---

        private IFormFile CreateMockFile(string fileName, long length)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(length);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.CopyTo(It.IsAny<Stream>()))
                .Callback<Stream>((stream) => stream.Write(new byte[1], 0, 1));
            return mockFile.Object;
        }

        private ProfessorPessoaModel GetNewProfessorModel()
        {
            return new ProfessorPessoaModel
            {
                Id = 3,
                Nome = "Novo Prof",
                Sobrenome = "Teste",
                Cpf = "11111111111",
                Email = "novo@prof.com",
                Senha = "123",
                Libras = false,
                IdCidade = 100, // ID da cidade de atuação (ProfessorPessoaDTO)

                // Campos Obrigatórios de PessoaDTO (Endereço, etc)
                Telefone = "11977777777",
                Genero = "Feminino",
                DataNascimento = new DateTime(1985, 5, 5),
                Cep = "01000000",
                Rua = "Rua do Professor",
                Numero = "123",
                Complemento = "Casa",
                Bairro = "Centro",
                Cidade = "São Paulo", // Cidade do Endereço (String)
                Estado = "SP"
            };
        }

        private ProfessorPessoaModel GetTargetProfessorModel()
        {
            return new ProfessorPessoaModel
            {
                Id = 1,
                Nome = "Professor Girafales",
                Sobrenome = "Madruga",
                Email = "girafales@escola.com",
                Senha = "", 
                Libras = true,
                IdCidade = 100, // ID da cidade de atuação

                // Campos Obrigatórios de PessoaDTO
                Telefone = "11988888888",
                Genero = "Masculino",
                DataNascimento = new DateTime(1980, 1, 1),
                Cep = "02000000",
                Rua = "Rua da Escola",
                Numero = "10",
                Complemento = "",
                Bairro = "Vila Madalena",
                Cidade = "São Paulo", // Cidade do Endereço
                Estado = "SP"
            };
        }

        private static Pessoa GetTargetProfessorEntity()
        {
            return new Pessoa
            {
                Id = 1,
                Nome = "Professor Girafales",
                Sobrenome = "Madruga",
                Email = "girafales@escola.com",
                Cpf = "12345678900",
                DescricaoProfessor = "Matemática",
                Libras = true,
                IdCidade = 100, // FK da cidade de atuação
                
                // Campos Obrigatórios de PessoaDTO/Entity
                Telefone = "11988888888",
                Genero = "Masculino",
                DataNascimento = new DateTime(1980, 1, 1),
                Cep = "02000000",
                Rua = "Rua da Escola",
                Numero = "10",
                Complemento = "",
                Bairro = "Vila Madalena",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private IEnumerable<Pessoa> GetTestProfessores()
        {
            return new List<Pessoa>
            {
                new Pessoa { 
                    Id = 1, 
                    Nome = "Professor Girafales", 
                    Sobrenome = "Madruga", 
                    Libras = true, 
                    IdCidade = 100,
                    // Campos básicos para evitar nulos se a View usar
                    Cidade = "São Paulo",
                    Telefone = "11988888888"
                },
                new Pessoa { 
                    Id = 2, 
                    Nome = "Professor Xavier", 
                    Sobrenome = "X-Men", 
                    Libras = false, 
                    IdCidade = 101,
                    Cidade = "Nova Iorque",
                    Telefone = "19999999999"
                }
            };
        }

        private IEnumerable<CidadeDTO> GetTestCidades()
        {
            return new List<CidadeDTO>
            {
                new CidadeDTO { Id = 100, Nome = "São Paulo" },
                new CidadeDTO { Id = 101, Nome = "Rio de Janeiro" }
            };
        }
    }
}