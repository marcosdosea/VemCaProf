
using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models;

public class AulaModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Horário início é obrigatório")]
    [Display(Name = "HorarioInicio")]
    public DateTime DataHorarioInicio { get; set; }

    [Required(ErrorMessage = "Horário final é obrigatório")]
    [Display(Name = "HorarioFinal")]
    public DateTime DataHorarioFinal { get; set; }

    [Required(ErrorMessage = "Descrição é obrigatório")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = null!;





    [Display(Name = "Status")]
    public string Status { get; set; } = null!;

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Display(Name = "Valor")]
    public double Valor { get; set; }


    [Display(Name = "DataHoraPagamento")]
    public DateTime DataHoraPagamento { get; set; }

    [Display(Name = "MetodoPagamento")]
    public string MetodoPagamento { get; set; } = null!;

    [Required(ErrorMessage = "IdDisciplina é obrigatório")]
    [Display(Name = "IdDisciplina")]
    public int IdDisciplina { get; set; }

    [Required(ErrorMessage = "IdResponsavel é obrigatório")]
    [Display(Name = "IdResponsavel")]
    public int IdResponsavel { get; set; }

    [Required(ErrorMessage = "IdAluno é obrigatório")]
    [Display(Name = "IdAluno")]
    public int IdAluno { get; set; }

    [Required(ErrorMessage = "IdProfessor é obrigatório")]
    [Display(Name = "IdProfessor")]
    public int IdProfessor { get; set; }

}
