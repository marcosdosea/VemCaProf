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
        [Range(0, 1, ErrorMessage = "Valor inválido para Atende em Libras.")]
        public bool AtendeLibras { get; set; }

        [Display(Name = "Descrição Profissional")]
        [StringLength(1000)]
        public string? Descricao { get; set; }
    }
}