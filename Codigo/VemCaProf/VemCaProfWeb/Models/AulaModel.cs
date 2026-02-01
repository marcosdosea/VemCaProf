
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
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Descrição deve ter entre 2 e 100 caracteres")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = null!;




}
