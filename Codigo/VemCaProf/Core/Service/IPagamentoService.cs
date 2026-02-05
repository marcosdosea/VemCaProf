using System.Collections.Generic;
using Core.DTO;

namespace Core.Service
{
    public interface IPagamentoService
    {
        IEnumerable<AulaDTO> ListarPagamentos();            // lista "pagamentos" (aulas)
        AulaDTO? BuscarPorAula(int idAula);                 // details
        bool RealizarPagamento(RealizarPagamentoDTO dto);   // pagar
    }
}
