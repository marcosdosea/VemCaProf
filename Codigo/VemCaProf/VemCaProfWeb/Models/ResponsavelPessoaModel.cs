using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models;

public class ResponsavelPessoaModel : PessoaModel
{
    [Display(Name = "Quantidade de Dependentes")]
    public int? QuantidadeDeDependentes { get; set; }
}