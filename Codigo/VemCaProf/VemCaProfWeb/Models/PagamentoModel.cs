using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models
{
    public class PagamentoModel
    {
        [Required]
        [Display(Name = "ID da Aula")]
        public int IdAula { get; set; }

        [Display(Name = "Descrição da Aula")]
        public string DescricaoAula { get; set; } = "";

        [Display(Name = "Valor")]
        [DataType(DataType.Currency)]
        public double Valor { get; set; }

        [Required]
        [Display(Name = "Método de Pagamento")]
        public string MetodoPagamento { get; set; } = null!;
    }
}