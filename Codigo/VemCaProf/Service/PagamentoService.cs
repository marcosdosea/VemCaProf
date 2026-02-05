using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.DTO;
using Core.Enums;
using Core.Service;

namespace Service
{
    public class PagamentoService : IPagamentoService
    {
        private readonly VemCaProfContext _context;

        public PagamentoService(VemCaProfContext context)
        {
            _context = context;
        }

        public IEnumerable<AulaDTO> ListarPagamentos()
        {
            // aqui você pode filtrar só pendentes: a.Status != StatusEnum.Paga
            return _context.Aulas
                .OrderByDescending(a => a.DataHoraPagamento)
                .Select(a => new AulaDTO
                {
                    Id = a.Id,
                    DataHorarioInicio = a.DataHorarioInicio,
                    DataHorarioFinal = a.DataHorarioFinal,
                    Descricao = a.Descricao,
                    Status = a.Status,
                    Valor = a.Valor,
                    DataHoraPagamento = a.DataHoraPagamento,
                    MetodoPagamento = a.MetodoPagamento,
                    IdDisciplina = a.IdDisciplina,
                    IdResponsavel = a.IdResponsavel,
                    IdAluno = a.IdAluno,
                    IdProfessor = a.IdProfessor
                })
                .ToList();
        }

        public AulaDTO? BuscarPorAula(int idAula)
        {
            var a = _context.Aulas.FirstOrDefault(x => x.Id == idAula);
            if (a == null) return null;

            return new AulaDTO
            {
                Id = a.Id,
                DataHorarioInicio = a.DataHorarioInicio,
                DataHorarioFinal = a.DataHorarioFinal,
                Descricao = a.Descricao,
                Status = a.Status,
                Valor = a.Valor,
                DataHoraPagamento = a.DataHoraPagamento,
                MetodoPagamento = a.MetodoPagamento,
                IdDisciplina = a.IdDisciplina,
                IdResponsavel = a.IdResponsavel,
                IdAluno = a.IdAluno,
                IdProfessor = a.IdProfessor
            };
        }

        public bool RealizarPagamento(RealizarPagamentoDTO dto)
        {
            if (dto.IdAula <= 0)
                throw new ServiceException("Aula inválida.");

            if (dto.MetodoPagamento != MetodoPagamentoEnum.Pix &&
                dto.MetodoPagamento != MetodoPagamentoEnum.Credito &&
                dto.MetodoPagamento != MetodoPagamentoEnum.Debito)
            {
                throw new ServiceException("Método de pagamento inválido.");
            }

            var aula = _context.Aulas.FirstOrDefault(a => a.Id == dto.IdAula);
            if (aula == null)
                throw new ServiceException("Aula não encontrada.");

            if (aula.Status == StatusEnum.Paga)
                throw new ServiceException("Esta aula já está paga.");

            // Não altera Valor (vem da aula)
            aula.MetodoPagamento = dto.MetodoPagamento;
            aula.DataHoraPagamento = DateTime.Now;
            aula.Status = StatusEnum.Paga;

            _context.SaveChanges();
            return true;
        }
    }
}
