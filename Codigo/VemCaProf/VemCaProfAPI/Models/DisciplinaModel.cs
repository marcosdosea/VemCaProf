using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models
{
    public class DisciplinaModel
    {
        [Display(Name = "Código")]
        [Key]
        [Required(ErrorMessage = "O campo Código é obrigatório.")]
        public uint Id { get; set; }

        [Display(Name = "Nome da disciplina")]
        [Required(ErrorMessage = "Campo requirido.")]
        [StringLength(45, MinimumLength = 5 ,ErrorMessage = "O nome da disciplina não pode exceder 45 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Descrição da disciplina")]
        [StringLength(200, ErrorMessage = "A descrição da disciplina não pode exceder 200 caracteres.")]
        public string? Descricao { get; set; }

        [Display(Name = "Nível da disciplina")]
        [StringLength(2, ErrorMessage = "O nível da disciplina deve ter no máximo 2 caracteres.")]
        public string? Nivel { get; set; }
    }
}
