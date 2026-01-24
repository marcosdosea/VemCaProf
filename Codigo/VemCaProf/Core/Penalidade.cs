using System;
using System.Collections.Generic;

namespace Core;

public partial class Penalidade
{
    public int Id { get; set; }

    public DateTime DataHorarioInicio { get; set; }

    public string Descricao { get; set; } = null!;

    public DateTime? DataHoraFim { get; set; }

    public string? Tipo { get; set; }

    public int IdProfessor { get; set; }

    public int IdResponsavel { get; set; }
}
