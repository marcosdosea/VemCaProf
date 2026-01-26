using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VemCaProfWeb.Models;

public class AlunoPessoaModel : PessoaModel
{
    public int IdResponsavel { get; set; }
    public IEnumerable<SelectListItem> ListaResponsaveis { get; set; }
        = Enumerable.Empty<SelectListItem>();
    
    [Display(Name = "Aluno de menor?")]
    [Required (ErrorMessage = "O campo AlunoDeMenor é obrigatório.")]
    [Range(0, 1, ErrorMessage = "Valor inválido para AlunoDeMenor.")]
    public bool? AlunoDeMenor { get; set; }
    
    [Display(Name = "Aluno Atípico?")]
    [Required (ErrorMessage = "O campo Atipico é obrigatório.")]
    [Range(0, 1, ErrorMessage = " Valor inválido para Atipico.")]
    public bool? Atipico { get; set; }
}