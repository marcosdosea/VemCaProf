using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models;

public class AlunoPessoaModel : PessoaModel
{
    [Display(Name = "Aluno de menor?")]
    [Required (ErrorMessage = "O campo AlunoDeMenor é obrigatório.")]
    [Range(0, 1, ErrorMessage = "Valor inválido para AlunoDeMenor.")]
    public bool? AlunoDeMenor { get; set; }
    
    [Display(Name = "Aluno Atípico?")]
    [Required (ErrorMessage = "O campo Atipico é obrigatório.")]
    [Range(0, 1, ErrorMessage = " Valor inválido para Atipico.")]
    public bool? Atipico { get; set; }
}