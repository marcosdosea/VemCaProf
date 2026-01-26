using System;
using System.Collections.Generic;

namespace Core;

public partial class DisponibilidadeHorario
{
    public int Id { get; set; }

    public DateTime Dia { get; set; }

    public TimeSpan HorarioInicio { get; set; }

    public TimeSpan HorarioFim { get; set; }

    public int IdProfessor { get; set; }

    public virtual Pessoa IdProfessorNavigation { get; set; } = null!;
}
