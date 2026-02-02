using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public class AlunoPessoaDTO : PessoaDTO
{
    public bool? AlunoDeMenor { get; set; }
    public bool? Atipico { get; set; }
    
    public int? ResponsavelId { get; set; }

}