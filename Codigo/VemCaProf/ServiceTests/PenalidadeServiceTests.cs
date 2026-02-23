using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    [TestClass]
    public class PenalidadeServiceTests
    {
        private VemCaProfContext context = null!;
        private IPessoaService pessoaService = null!;
        private IPenalidadeService penalidadeService = null!;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<VemCaProfContext>();
            builder.UseInMemoryDatabase("VCP" + Guid.NewGuid().ToString());
            var options = builder.Options;

            context = new VemCaProfContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var pessoas = new List<Pessoa>
            {
                // ID 1: Professor
                new Pessoa
                {
                    Id = 1,
                    Nome = "Professor Juninho",
                    Sobrenome = "Silva",
                    Cpf = "12345678901",
                    Email = "juju@email.com",
                    DescricaoProfessor = "Matemática",
                    TipoPessoa = "P",
                    Libras = true,
                    IdCidade = 1,
                    Genero = "Masculino",
                    Complemento = "Casa X",
                    Telefone = "11999999999",
                    Cep = "00000-000",
                    Rua = "Rua A",
                    Numero = "1",
                    Bairro = "B",
                    Cidade = "SP",
                    Estado = "SP",
                    DataNascimento = new DateTime(1970, 1, 1)
                    },
                // ID 2: Aluno
                new Pessoa
                {
                    Id = 2,
                    Nome = "Maria",
                    Sobrenome = "Silva",
                    Cpf = "44444444444",
                    Email = "maria.silva@email.com",
                    AlunoDeMenor = false,
                    Atipico = true,
                    TipoPessoa = "A",
                    Genero = "Feminino",
                    Complemento = "Apartamento 12",
                    Telefone = "11999999999",
                    Cep = "12345-678",
                    Rua = "Rua das Flores",
                    Numero = "45",
                    Bairro = "Centro",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    DataNascimento = new DateTime(1995, 3, 20)
                },
                // ID 3: Responsável
                new Pessoa
                {
                    Id = 3,
                    Nome = "João",
                    Sobrenome = "Pereira",
                    Cpf = "55555555555",
                    Email = "joao.pereira@email.com",
                    QuantidadeDeDependentes = 2,
                    TipoPessoa = "R",
                    Genero = "Masculino",
                    Complemento = "Casa 7",
                    Telefone = "11888888888",
                    Cep = "98765-432",
                    Rua = "Avenida Brasil",
                    Numero = "100",
                    Bairro = "Jardim",
                    Cidade = "Aracaju",
                    Estado = "SE",
                    DataNascimento = new DateTime(1988, 12, 15)
                }
            };

            context.AddRange(pessoas);
            context.SaveChanges();

            // Seed inicial de penalidades
            var penalidades = new List<Penalidade>
            {
                new Penalidade
                {
                    Id = 1,
                    DataHorarioInicio = new DateTime(2025, 1, 1, 9, 0, 0),
                    DataHoraFim = new DateTime(2025, 1, 1, 10, 0, 0),
                    Tipo = "A",
                    Descricao = "Primeira penalidade",
                    IdProfessor = 1,
                    IdResponsavel = 3
                },
                new Penalidade
                {
                    Id = 2,
                    DataHorarioInicio = new DateTime(2025, 2, 2, 14, 0, 0),
                    DataHoraFim = new DateTime(2025, 2, 2, 15, 0, 0),
                    Tipo = "B",
                    Descricao = "Segunda penalidade",
                    IdProfessor = 1,
                    IdResponsavel = 3
                }
            };

            context.AddRange(penalidades);
            context.SaveChanges();


            pessoaService = new PessoaService(context);
            penalidadeService = new PenalidadeService(context, pessoaService);
        }


        //-- Create tests --

        [TestMethod]
        public void CreatePenalidade_Success()
        {
            var nova = new Penalidade
            {
                DataHorarioInicio = new DateTime(2025, 3, 3, 10, 0, 0),
                DataHoraFim = new DateTime(2025, 3, 3, 11, 0, 0),
                Tipo = "C",
                Descricao = "Teste create",
                IdProfessor = 1,
                IdResponsavel = 3
            };

            var id = penalidadeService.Create(nova);

            var criado = context.Penalidades.Find(id);
            Assert.IsNotNull(criado);
            Assert.AreEqual("Teste create", criado.Descricao);
            Assert.AreEqual(1, criado.IdProfessor);
            Assert.AreEqual(3, criado.IdResponsavel);
        }

        [TestMethod]
        public void CreatePenalidade_InvalidProfessor()
        {
            var nova = new Penalidade
            {
                DataHorarioInicio = new DateTime(2025, 4, 1, 9, 0, 0),
                DataHoraFim = new DateTime(2025, 4, 1, 10, 0, 0),
                Tipo = "C",
                Descricao = "Professor inválido",
                IdProfessor = 999,
                IdResponsavel = 3
            };

            Assert.ThrowsException<ServiceException>(() => penalidadeService.Create(nova));
        }

        [TestMethod]
        public void CreatePenalidade_InvalidResponsible()
        {
            var nova = new Penalidade
            {
                DataHorarioInicio = new DateTime(2025, 4, 1, 9, 0, 0),
                DataHoraFim = new DateTime(2025, 4, 1, 10, 0, 0),
                Tipo = "C",
                Descricao = "Responsável inválido",
                IdProfessor = 1,
                IdResponsavel = 999
            };

            Assert.ThrowsException<ServiceException>(() => penalidadeService.Create(nova));
        }

        [TestMethod]
        public void CreatePenalidade_InvalidDates()
        {
            var nova = new Penalidade
            {
                DataHorarioInicio = new DateTime(2025, 5, 1, 11, 0, 0),
                DataHoraFim = new DateTime(2025, 5, 1, 10, 0, 0), // fim antes do início
                Tipo = "C",
                Descricao = "Datas inválidas",
                IdProfessor = 1,
                IdResponsavel = 3
            };

            Assert.ThrowsException<ServiceException>(() => penalidadeService.Create(nova));
        }
        // -- Edit tests --
        [TestMethod]
        public void EditPenalidade_Success()
        {
            var existente = context.Penalidades.Find(1)!;
            existente.Descricao = "Alterada";
            existente.DataHorarioInicio = existente.DataHorarioInicio.AddHours(1);
            existente.DataHoraFim = existente.DataHoraFim?.AddHours(1);

            penalidadeService.Edit(existente);

            var editado = penalidadeService.Get(1);
            Assert.AreEqual("Alterada", editado.Descricao);
            Assert.AreEqual(existente.DataHorarioInicio, editado.DataHorarioInicio);
        }

        [TestMethod]
        public void EditPenalidade_InvalidDates()
        {
            var existente = context.Penalidades.Find(2)!;
            existente.DataHorarioInicio = new DateTime(2025, 2, 2, 16, 0, 0);
            existente.DataHoraFim = new DateTime(2025, 2, 2, 15, 0, 0);

            Assert.ThrowsException<ServiceException>(() => penalidadeService.Edit(existente));
        }

        // -- Delete tests --

        [TestMethod]
        public void DeletePenalidade_Success()
        {
            penalidadeService.Delete(2);
            Assert.AreEqual(1, context.Penalidades.Count());
            Assert.IsNull(context.Penalidades.Find(2));
        }


        [TestMethod]
        public void DeletePenalidade_NotFind()
        {
            Assert.ThrowsException<ServiceException>(() => penalidadeService.Delete(9999));
            
        }

        // -- Get tests --

        [TestMethod]
        public void GetPenalidade_Success()
        {
            var p = penalidadeService.Get(1);
            Assert.IsNotNull(p);
            Assert.AreEqual("Primeira penalidade", p.Descricao);
        }

        [TestMethod]
        public void GetPenalidade_Invalid()
        {
            var p = penalidadeService.Get(9999);
            Assert.IsNull(p);

        }

        [TestMethod]
        public void GetByPessoaId_ReturnsPenalidades()
        {
            var lista = penalidadeService.GetByPessoaId(1).ToList();
            Assert.IsTrue(lista.Count >= 1);
            Assert.IsTrue(lista.All(p => p.IdProfessor == 1 || p.IdResponsavel == 1));
        }

        [TestMethod]
        public void GetByPessoaId_NoPenalidade()
        {

            Assert.ThrowsException<ServiceException>(() => penalidadeService.GetByPessoaId(999).ToList());


        }

    }
}
