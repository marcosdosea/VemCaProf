using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models
{
    public class ProfessorPessoaModel : PessoaModel
    {
        
        [Display(Name = "Cidade de Atuação")]
        [Required(ErrorMessage = "Selecione ao menos uma cidade.")]
        public int IdCidade { get; set; }
        
        public IEnumerable<SelectListItem> ListaDeCidades { get; set; }
            = Enumerable.Empty<SelectListItem>();

        [Display(Name = "Atende em Libras?")]
        public bool Libras { get; set; }

        [Display(Name = "Descrição Professor")]
        [StringLength(1000)]
        public string? DescricaoProfessor { get; set; }
    }
}