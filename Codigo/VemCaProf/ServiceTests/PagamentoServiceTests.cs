using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.DTO;
using Core.Enums;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Service.Tests
{
    [TestClass]
    public class PagamentoServiceTests
    {
        private VemCaProfContext context = null!;
        private IPagamentoService pagamentoService = null!;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<VemCaProfContext>();
            builder.UseInMemoryDatabase($"VCP_{Guid.NewGuid()}");
            var options = builder.Options;

            context = new VemCaProfContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed de aulas (1 pendente e 1 paga)
            var aulas = new List<Aula>
            {
                new Aula
                {
                    Id = 1,
                    Descricao = "Aula Pendente",
                    DataHorarioInicio = new DateTime(2026, 02, 10, 14, 0, 0),
                    DataHorarioFinal = new DateTime(2026, 02, 10, 15, 0, 0),
                    Valor = 120.0,
                    MetodoPagamento = MetodoPagamentoEnum.Pix,       // placeholder
                    Status = StatusEnum.AguardandoPagamento,        // AP
                    DataHoraPagamento = new DateTime(2000, 01, 01), // placeholder
                    IdDisciplina = 1,
                    IdResponsavel = 1,
                    IdAluno = 2,
                    IdProfessor = 3
                },
                new Aula
                {
                    Id = 2,
                    Descricao = "Aula Paga",
                    DataHorarioInicio = new DateTime(2026, 02, 11, 14, 0, 0),
                    DataHorarioFinal = new DateTime(2026, 02, 11, 15, 0, 0),
                    Valor = 100.0,
                    MetodoPagamento = MetodoPagamentoEnum.Credito,
                    Status = StatusEnum.Paga,                       // PG
                    DataHoraPagamento = new DateTime(2026, 02, 05, 9, 0, 0),
                    IdDisciplina = 1,
                    IdResponsavel = 1,
                    IdAluno = 2,
                    IdProfessor = 3
                }
            };

            context.Aulas.AddRange(aulas);
            context.SaveChanges();

            pagamentoService = new PagamentoService(context);
        }

        [TestMethod]
        public void ListarPagamentos_DeveRetornarLista()
        {
            var lista = pagamentoService.ListarPagamentos().ToList();

            Assert.IsNotNull(lista);
            Assert.AreEqual(2, lista.Count);

            Assert.IsTrue(lista.Any(a => a.Descricao == "Aula Pendente"));
            Assert.IsTrue(lista.Any(a => a.Descricao == "Aula Paga"));
        }

        [TestMethod]
        public void BuscarPorAula_Existe_DeveRetornarDTO()
        {
            var dto = pagamentoService.BuscarPorAula(1);

            Assert.IsNotNull(dto);
            Assert.AreEqual(1, dto.Id);
            Assert.AreEqual("Aula Pendente", dto.Descricao);
            Assert.AreEqual(StatusEnum.AguardandoPagamento, dto.Status);
        }

        [TestMethod]
        public void BuscarPorAula_NaoExiste_DeveRetornarNull()
        {
            var dto = pagamentoService.BuscarPorAula(999);
            Assert.IsNull(dto);
        }

        [TestMethod]
        public void RealizarPagamento_Valido_DeveAtualizarStatusEData()
        {
            var antes = context.Aulas.First(a => a.Id == 1);
            Assert.AreEqual(StatusEnum.AguardandoPagamento, antes.Status);

            var dto = new RealizarPagamentoDTO
            {
                IdAula = 1,
                MetodoPagamento = MetodoPagamentoEnum.Debito
            };

            var ok = pagamentoService.RealizarPagamento(dto);
            Assert.IsTrue(ok);

            var depois = context.Aulas.First(a => a.Id == 1);
            Assert.AreEqual(StatusEnum.Paga, depois.Status);
            Assert.AreEqual(MetodoPagamentoEnum.Debito, depois.MetodoPagamento);

            // DataHoraPagamento deve ter sido atualizada (não deve ficar com placeholder)
            Assert.IsTrue(depois.DataHoraPagamento > new DateTime(2000, 01, 01));
        }

        [TestMethod]
        public void RealizarPagamento_AulaJaPaga_DeveLancarServiceException()
        {
            var dto = new RealizarPagamentoDTO
            {
                IdAula = 2,
                MetodoPagamento = MetodoPagamentoEnum.Pix
            };

            Assert.ThrowsException<ServiceException>(() => pagamentoService.RealizarPagamento(dto));
        }

        [TestMethod]
        public void RealizarPagamento_AulaInexistente_DeveLancarServiceException()
        {
            var dto = new RealizarPagamentoDTO
            {
                IdAula = 999,
                MetodoPagamento = MetodoPagamentoEnum.Pix
            };

            Assert.ThrowsException<ServiceException>(() => pagamentoService.RealizarPagamento(dto));
        }

        [TestMethod]
        public void RealizarPagamento_MetodoInvalido_DeveLancarServiceException()
        {
            var dto = new RealizarPagamentoDTO
            {
                IdAula = 1,
                MetodoPagamento = "X"
            };

            Assert.ThrowsException<ServiceException>(() => pagamentoService.RealizarPagamento(dto));
        }
    }
}
