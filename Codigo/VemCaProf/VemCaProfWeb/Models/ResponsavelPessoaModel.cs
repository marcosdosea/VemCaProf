using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models;

public class ResponsavelPessoaModel : PessoaModel
{
    [Display(Name = "Quantidade de Dependentes")]
    [Range(0, 20, ErrorMessage = "Quantidade inválida")]
    public int? QuantidadeDeDependentes { get; set; }
}