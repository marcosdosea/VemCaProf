
using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models;

public class DisponibilidadeHorarioModel
{
    public int Id { get; set; }

    public DateTime Dia { get; set; }

    [Required(ErrorMessage = "Horário início é obrigatório")]
    [Display(Name = "Horário início")]
    [DataType(DataType.Time)]
    public TimeSpan HorarioInicio { get; set; }


    [Required(ErrorMessage = "Horário fim é obrigatório")]
    [Display(Name = "Horário fim")]
    [DataType(DataType.Time)]
    public TimeSpan HorarioFim { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione um professor")]
    [Display(Name = "Professor")]
    public int IdProfessor { get; set; }

    [Display(Name = "Professor")]
    public string? NomeProfessor { get; set; }

}
