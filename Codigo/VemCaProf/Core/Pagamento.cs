using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public int Id { get; set; }

    public double Valor { get; set; }

    public DateTime DataHora { get; set; }

    public string Status { get; set; } = null!;

    public int IdResponsavelPagamento { get; set; }
}
