using System;
using System.Collections.Generic;

namespace Core;

public partial class Aula
{
    public int Id { get; set; }

    public DateTime DataHorarioInicio { get; set; }

    public DateTime DataHorarioFinal { get; set; }

    public string Descricao { get; set; } = null!;


    public DateTime DataHoraPagamento { get; set; }

    public double Valor { get; set; }

    /// <summary>
    /// P = Pix
    /// C = Credito
    /// D = Debito
    /// </summary>
    public string MetodoPagamento { get; set; } = null!;

    /// <summary>
    /// AG = Agendada
    /// RE = Realizada
    /// PG = Paga
    /// AP = Aguardando Pagamento
    /// CA = Cancelada
    /// CO = Confirmada
    /// 
    /// </summary>
    public string Status { get; set; } = null!;

    public uint IdDisciplina { get; set; }

    public int IdResponsavel { get; set; }

    public int IdAluno { get; set; }

    public int IdProfessor { get; set; }


    public virtual Pessoa IdAlunoNavigation { get; set; } = null!;

    public virtual Disciplina IdDisciplinaNavigation { get; set; } = null!;

    public virtual Pessoa IdProfessorNavigation { get; set; } = null!;

    public virtual Pessoa IdResponsavelNavigation { get; set; } = null!;
}
