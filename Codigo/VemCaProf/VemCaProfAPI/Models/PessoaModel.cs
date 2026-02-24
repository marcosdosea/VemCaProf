using System.ComponentModel.DataAnnotations;
using VemCaProfAPI.Models; 


namespace VemCaProfAPI.Models
{
    public class PessoaApiModel
    {
        // Identificador (usado apenas em respostas e no PUT)
        public int Id { get; set; }

        // Dados pessoais obrigatórios
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O sobrenome é obrigatório.")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Telefone inválido.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O gênero é obrigatório.")]
        public string Genero { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        // Endereço
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve ter 8 dígitos.")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "A rua é obrigatória.")]
        public string Rua { get; set; }

        [Required(ErrorMessage = "O número é obrigatório.")]
        public string Numero { get; set; }

        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O estado deve ter 2 caracteres.")]
        public string Estado { get; set; }

        // Tipo de pessoa (P, A, R)
        [Required(ErrorMessage = "O tipo de pessoa é obrigatório.")]
        [RegularExpression("^[PAR]$", ErrorMessage = "Tipo de pessoa deve ser P (Professor), A (Aluno) ou R (Responsável).")]
        public string TipoPessoa { get; set; }

        // Campos específicos para Professor
        public string? DescricaoProfessor { get; set; }
        public bool Libras { get; set; }
        public int? IdCidade { get; set; }
        public List<int>? IdDisciplinas { get; set; } // Lista de IDs das disciplinas

        // Campos específicos para Aluno
        public bool AlunoDeMenor { get; set; }
        public bool Atipico { get; set; }
        public int? ResponsavelId { get; set; }

        // Campos específicos para Responsável
        public int? QuantidadeDeDependentes { get; set; }
    }
}