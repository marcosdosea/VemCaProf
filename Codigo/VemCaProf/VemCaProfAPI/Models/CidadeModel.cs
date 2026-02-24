using System.ComponentModel.DataAnnotations;

namespace VemCaProfAPI.Models
{
    public class CidadeModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(45, ErrorMessage = "Nome deve ter no máximo 45 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Estado é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres.")]
        public string Estado { get; set; } = string.Empty;
    }
}