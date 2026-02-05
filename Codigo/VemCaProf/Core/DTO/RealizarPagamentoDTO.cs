using System;

namespace Core.DTO
{
    public class RealizarPagamentoDTO
    {
        public int IdAula { get; set; }
        public string MetodoPagamento { get; set; } = null!;
    }
}
