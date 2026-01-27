
using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models;

public class DisponibilidadeHorarioModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Dia é obrigatório")]
    [Display(Name = "Dia")]
    public DateTime Dia { get; set; }

    [Required(ErrorMessage = "Horário início é obrigatório")]
    [Display(Name = "Horário início")]
    public TimeSpan HorarioInicio { get; set; }


    [Required(ErrorMessage = "Horário fim é obrigatório")]
    [Display(Name = "Horário fim")]
    public TimeSpan HorarioFim { get; set; }

    [Required(ErrorMessage = "Código do professor é obrigatório")]
    [Display(Name = "Código do professor")]
    public int IdProfessor { get; set; }

}