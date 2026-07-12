using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Moq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using VemCaProfWeb.Controllers;
using VemCaProfWeb.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VemCaProfWebTests.Controllers
{
    [TestClass()]
    public class PenalidadeControllerTests
    {
        private static PenalidadeController controller = null!;
        private Mock<IPenalidadeService> mockService = null!;
      
        private static IMapper _mapper = null!;
        private ILogger<PenalidadeController> _logger = null!;

        [TestInitialize]
        public void Initialize()
        {
            Mock<IPessoaService> _pessoaService = null!;
        mockService = new Mock<IPenalidadeService>();
            _pessoaService = new Mock<IPessoaService>();
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PenalidadeDTO, PenalidadeModel>().ReverseMap();
            }).CreateMapper();

            mockService.Setup(Service => Service.GetAll()).Returns(GetAll());
            mockService.Setup(service => service.Get(1)).Returns(GetPenalidade());
            

            _logger = new Mock<ILogger<PenalidadeController>>().Object;
            controller = new PenalidadeController(mockService.Object, _pessoaService.Object, _mapper, _logger);
        }

        [TestMethod]
        public void IndexTest_Valido()
        {
            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<PenalidadeModel>));

            var lista = (IEnumerable<PenalidadeModel>)viewResult.Model;
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod()]
        public void DetailsTest_Valido()
        {
            // Act
            var result = controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.Model);

            Assert.IsInstanceOfType(viewResult.Model, typeof(PenalidadeModel));
            var penalidadeModel = (PenalidadeModel)viewResult.Model;
            Assert.AreEqual(1, penalidadeModel.Id);
            Assert.AreEqual("Atraso", penalidadeModel.Tipo);
            Assert.AreEqual(1, penalidadeModel.IdProfessor);
        }

        [TestMethod()]
        public void CreateTest_Valido()
        {
            
            var result = controller.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        private IEnumerable<PenalidadeDTO> GetAll()
        {
            return new List<PenalidadeDTO>
            {
                new PenalidadeDTO
                {
                    Id = 1,
                    Tipo = "Atraso",
                    DataHorarioInicio = DateTime.Now.AddDays(-1),
                    Descricao = "Atraso de 15 minutos",
                    IdProfessor = 1,
                    IdResponsavel = 2
                },
                new PenalidadeDTO
                {
                    Id = 2,
                    Tipo = "Falta",
                    DataHorarioInicio = DateTime.Now.AddDays(-2),
                    Descricao = "Falta sem aviso prévio",
                    IdProfessor = 1,
                    IdResponsavel = 2
                },
                new PenalidadeDTO
                {
                    Id = 3,
                    Tipo = "Conduta indevida",
                    DataHorarioInicio = DateTime.Now.AddDays(-2),
                    Descricao = "O professor não seguiu as diretrizes",
                    IdProfessor = 2,
                    IdResponsavel = 2
                },
            };

        }

        private PenalidadeDTO GetPenalidade()
        {
            return new PenalidadeDTO
            {
                Id = 1,
                Tipo = "Atraso",
                DataHorarioInicio = DateTime.Now.AddDays(-1),
                Descricao = "Atraso de 15 minutos",
                IdProfessor = 1,
                IdResponsavel = 2
            };
        }

        private Pessoa GetProfessor()
        {
            return new Pessoa 
            {
                Id = 1,
                Nome = "Professor",
                Sobrenome = "Girafales",
                Cpf = "111.111.111-11",
                Email = "girafales@escola.com",
                Telefone = "(11) 99999-1111",
                Genero = "Masculino",
                DataNascimento = new DateTime(1980, 5, 20),
                Cep = "01001-000",
                Rua = "Praça da Sé",
                Numero = "123",
                Complemento = "Sala 4",
                Bairro = "Sé",
                Cidade = "São Paulo",
                Estado = "SP"
            };
        }

        private List<Pessoa> GetAllResponsavel()
        {
            return new List<Pessoa>
            {
                new Pessoa
                {
                    Id = 2,
                    Nome = "Responsável",
                    Sobrenome = "Seu Madruga",
                    Cpf = "222.222.222-22",
                    Email = "madruga@vizinhanca.com",
                    Telefone = "(11) 98888-2222",
                    Genero = "Masculino",
                    DataNascimento = new DateTime(1975, 3, 15),
                    Cep = "01310-100",
                    Rua = "Avenida Paulista",
                    Numero = "71",
                    Complemento = "Casa 72",
                    Bairro = "Bela Vista",
                    Cidade = "São Paulo",
                    Estado = "SP"
                },
                new Pessoa
                {
                    Id = 3,
                    Nome = "Responsável3",
                    Sobrenome = "Seu Madruga3",
                    Cpf = "222.222.222-33",
                    Email = "madruga3@vizinhanca.com",
                    Telefone = "(11) 98888-2222",
                    Genero = "Masculino",
                    DataNascimento = new DateTime(1975, 3, 15),
                    Cep = "01310-100",
                    Rua = "Avenida Paulista",
                    Numero = "71",
                    Complemento = "Casa 72",
                    Bairro = "Bela Vista",
                    Cidade = "São Paulo",
                    Estado = "SP"
                },
            };
        }

        private List<Pessoa> GetAllProfessor()
        {
            return new List<Pessoa>
            {
                new Pessoa
            {
                Id = 1,
                Nome = "Professor",
                Sobrenome = "Girafales",
                Cpf = "111.111.111-11",
                Email = "girafales@escola.com",
                Telefone = "(11) 99999-1111",
                Genero = "Masculino",
                DataNascimento = new DateTime(1980, 5, 20),
                Cep = "01001-000",
                Rua = "Praça da Sé",
                Numero = "123",
                Complemento = "Sala 4",
                Bairro = "Sé",
                Cidade = "São Paulo",
                Estado = "SP"
            },
                new Pessoa
            {
                Id = 2,
                Nome = "Professor2",
                Sobrenome = "Girafales2",
                Cpf = "222.222.222-22",
                Email = "girafales2@escola.com",
                Telefone = "(11) 99999-1111",
                Genero = "Masculino",
                DataNascimento = new DateTime(1980, 5, 20),
                Cep = "01001-000",
                Rua = "Praça da Sé",
                Numero = "123",
                Complemento = "Sala 4",
                Bairro = "Sé",
                Cidade = "São Paulo",
                Estado = "SP"
            }

            };
            

        }

    }
}
