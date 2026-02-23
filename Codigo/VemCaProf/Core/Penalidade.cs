using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

    [NotMapped]
    public string NomeProfessor { get; set; } = null!;

    [NotMapped]
    public string NomeResponsavel { get; set;}  = null!;
    public virtual Pessoa IdProfessorNavigation { get; set; } = null!;

    public virtual Pessoa IdResponsavelNavigation { get; set; } = null!;
}
