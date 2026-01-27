
using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models;

public class CidadeModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = null!;

    [Required(ErrorMessage = "Estado é obrigatório")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres")]
    [RegularExpression(@"[A-Z]{2}", ErrorMessage = "Estado deve conter apenas letras maiúsculas")]
    [Display(Name = "Estado")]
    public string Estado { get; set; } = null!;
}