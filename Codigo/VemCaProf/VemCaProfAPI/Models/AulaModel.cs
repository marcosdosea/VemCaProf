
using System.ComponentModel.DataAnnotations;

namespace VemCaProfAPI.Models;

public class AulaModel
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um horário")]
    public int IdDisponibilidadeHorario { get; set; }

    [Required(ErrorMessage = "A data da aula é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime DataAula { get; set; }

    [Required(ErrorMessage = "Descrição é obrigatório")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = null!;





    [Required(ErrorMessage = "Valor é obrigatório")]
    [Display(Name = "Valor")]
    public double Valor { get; set; }


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

}
