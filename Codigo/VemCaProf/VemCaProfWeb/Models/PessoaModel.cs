using System.ComponentModel.DataAnnotations;

namespace VemCaProfWeb.Models
{
    public class PessoaModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O Sobrenome é obrigatório")]
        public string? Sobrenome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        public string? Cpf { get; set; } 

        [Required(ErrorMessage = "O email é obrigatório")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [DataType(DataType.PhoneNumber)]
        public string? Telefone { get; set; }
        
        [Required(ErrorMessage = "O gênero é obrigatório")]
        public string? Genero { get; set; }
        
        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório")]
        public string? Cep { get; set; }
        
        [Required(ErrorMessage = "A rua é obrigatória")]
        public string? Rua { get; set; }
        
        [Required(ErrorMessage = "O número é obrigatório")]
        public string? Numero { get; set; }
        
        [Required(ErrorMessage = "O complemento é obrigatório")]
        public string? Complemento { get; set; }
        
        [Required(ErrorMessage = "O bairro é obrigatório")]
        public string? Bairro { get; set; }
        
        [Required(ErrorMessage = "A cidade é obrigatória")]
        public string? Cidade { get; set; }
        
        [Required(ErrorMessage = "O estado é obrigatório")]
        public string? Estado { get; set; }

        public string? TipoPessoa { get; set; } 
        
        // --- PROFESSOR ---
        public string? DescricaoProfessor { get; set; }
        public bool Libras { get; set; }
        public int? IdCidade { get; set; } 
        public List<uint>? DisciplinasSelecionadas { get; set; } = new List<uint>();
        
        public byte[]? FotoPerfil { get; set; }
        public byte[]? Diploma { get; set; }
        public byte[]? FotoDocumento { get; set; }

        // --- ALUNO ---
        public bool AlunoDeMenor { get; set; }
        public bool Atipico { get; set; }
        public int? ResponsavelId { get; set; } 

        // --- RESPONSÁVEL ---
        public int? QuantidadeDeDependentes { get; set; }
    }
}