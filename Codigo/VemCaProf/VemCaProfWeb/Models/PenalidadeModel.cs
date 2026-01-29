using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models
{
    public class PenalidadeModel
    {
        [Display(Name = "Código da penalidade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int Id { get; set; }

        [Display(Name = "Data e hora de início")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.DateTime, ErrorMessage = "O campo deve apresentar uma data e hora válida")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DataHorarioInicio { get; set; }

        [Display(Name = "Data e hora de fim")]

        [DataType(DataType.DateTime,ErrorMessage = "O campo deve apresentar uma data e hora válida")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataHoraFim { get; set; }

        [Display(Name = "Tipo de penalidade")]
        public string? Tipo { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = null!;
    }
}
