using System;
using System.Collections.Generic;

namespace Core;

public partial class Aula
{
    public int Id { get; set; }

    public DateTime DataHorarioInicio { get; set; }

    public DateTime DataHorarioFinal { get; set; }

    public string Descricao { get; set; } = null!;

    public int IdAulaPagamento { get; set; }

    public int IdDisciplinaAula { get; set; }

    public int IdResponsavelAula { get; set; }

    public int IdProfessorAula { get; set; }
}
