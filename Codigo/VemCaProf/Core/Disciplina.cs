using System;
using System.Collections.Generic;

namespace Core;

public partial class Disciplina
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    /// <summary>
    /// F1 = Ensino Fundamental Menor
    /// F2 = Ensino Fundamental Maior
    /// M1 = Ensino Medio 
    /// </summary>
    public string? Nivel { get; set; }
}
