using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public class AlunoPessoaDTO : PessoaDTO
{
    [Required (ErrorMessage = "O campo AlunoDeMenor é obrigatório.")]
    public bool? AlunoDeMenor { get; set; }
    [Required (ErrorMessage = "O campo Atipico é obrigatório.")]
    public bool? Atipico { get; set; }

}