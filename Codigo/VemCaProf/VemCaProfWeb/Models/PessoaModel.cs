using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models
{
    public class PessoaModel
    {
        
        [Display(Name = "Código")]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [Display(Name = "Sobrenome")]
        public string Sobrenome { get; set; } = null!;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = null!;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Digite um e-mail válido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = null!;

        // Endereço e outros campos comuns...
        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20,ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        [Display(Name = "Telefone / WhatsApp")]
        public string? Telefone { get; set; }
        
        [Required(ErrorMessage = "O gênero é obrigatório")]
        [Display(Name = "Gênero")]
        public string Genero { get; set; } = null!;
        
        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }
    
        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(8, ErrorMessage = "O CEP deve conter 8 caracteres")]
        [Display(Name = "CEP")]
        public string Cep { get; set; } = null!;
        
        [Required(ErrorMessage = "A rua é obrigatória")]
        public string Rua { get; set; } = null!;
        
        [Required(ErrorMessage = "O número é obrigatório")]
        public string Numero { get; set; } = null!;
        
        [Required(ErrorMessage = "O complemento é obrigatório")]
        public string Complemento { get; set; } = null!;
        
        [Required(ErrorMessage = "O bairro é obrigatório")]
        public string Bairro { get; set; } = null!;
        
        [Required(ErrorMessage = "A cidade é obrigatória")]
        public string Cidade { get; set; } = null!;
        
        [Required(ErrorMessage = "O estado é obrigatório")]
        public string Estado { get; set; } = null!;
    }
}