
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

    [Required(ErrorMessage = "A data da aula é obrigatória")]
    [DataType(DataType.Date)]
    [Display(Name = "Data da aula")]
    public DateTime? DataAula { get; set; }

    [Required(ErrorMessage = "Descrição é obrigatório")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = null!;





    [Display(Name = "Status")]
    public string? Status { get; set; }

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Display(Name = "Valor")]
    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public double? Valor { get; set; }


    [Display(Name = "DataHoraPagamento")]
    public DateTime? DataHoraPagamento { get; set; }

    [Display(Name = "MetodoPagamento")]
    public string? MetodoPagamento { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um horário")]
    [Display(Name = "Horário")]
    public int IdDisponibilidadeHorario { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma disciplina")]
    [Display(Name = "Disciplina")]
    public int IdDisciplina { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um responsável")]
    [Display(Name = "Responsável")]
    public int IdResponsavel { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um aluno")]
    [Display(Name = "Aluno")]
    public int IdAluno { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um professor")]
    [Display(Name = "Professor")]
    public int IdProfessor { get; set; }

    [Display(Name = "Disciplina")]
    public string? NomeDisciplina { get; set; }

    [Display(Name = "Responsável")]
    public string? NomeResponsavel { get; set; }

    [Display(Name = "Aluno")]
    public string? NomeAluno { get; set; }

    [Display(Name = "Professor")]
    public string? NomeProfessor { get; set; }

}
