using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VemCaProfWeb.Models;

public class AlunoPessoaModel : PessoaModel
{
    
    [Display(Name = "Responsável")]
    public int? IdResponsavel { get; set; }
    
    public IEnumerable<SelectListItem> ListaResponsaveis { get; set; }
        = Enumerable.Empty<SelectListItem>();
    
    [Display(Name = "Aluno de menor?")]
    public bool AlunoDeMenor { get; set; }
    
    [Display(Name = "Aluno Atípico?")]
    public bool Atipico { get; set; }
    
}