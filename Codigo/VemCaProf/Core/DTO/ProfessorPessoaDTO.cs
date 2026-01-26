using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public class ProfessorPessoaDTO : PessoaDTO
{
    public byte[]? Diploma { get; set; }

    public byte[]? FotoDocumento { get; set; }

    public byte[]? FotoPerfil { get; set; }

    public int IdCidade { get; set; }
    
    public bool? Libras { get; set; }
    
    public string? DescricaoProfessor { get; set; }

}
