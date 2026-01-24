using System;
using System.Collections.Generic;

namespace Core;

public partial class Disciplina
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public string? Nivel { get; set; }
}
