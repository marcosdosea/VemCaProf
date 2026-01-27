using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models
{
    public class PenalidadeModel
    {
        [Display(Name = "Código da penalidade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int Id { get; set; }
    }
}
