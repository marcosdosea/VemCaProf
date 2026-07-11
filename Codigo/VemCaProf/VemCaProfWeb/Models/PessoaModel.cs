using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;

namespace VemCaProfWeb.Models
{
    public class PessoaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O Sobrenome é obrigatório")]
        public string? Sobrenome { get; set; }

        //[CPF(ErrorMessage = "CPF inválido")]
        [Required]
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

        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório")]
        public string? Bairro { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage = "O estado é obrigatório")]
        public string? Estado { get; set; }

        [Required(ErrorMessage = "Tipo de pessoa é obrigatório")]
        public string? TipoPessoa { get; set; }

        // Arquivos (byte[] para exibição)
        public byte[]? FotoPerfil { get; set; }
        public byte[]? Diploma { get; set; }
        public byte[]? FotoDocumento { get; set; }

        // Arquivos para upload (IFormFile)
        public IFormFile? ArquivoFotoPerfil { get; set; }
        public IFormFile? ArquivoDiploma { get; set; }
        public IFormFile? ArquivoFotoDocumento { get; set; }

        // Professor
        public string? DescricaoProfessor { get; set; }
        public bool Libras { get; set; }
        public int? IdCidade { get; set; }
        public List<int>? IdDisciplinas { get; set; } // IDs selecionados

        // Aluno
        public bool AlunoDeMenor { get; set; }
        public bool Atipico { get; set; }
        public int? ResponsavelId { get; set; }
        public string? NomeResponsavel { get; set; }

        // Responsável
        public int? QuantidadeDeDependentes { get; set; }

        // Listas para exibição (Details)
        public List<string>? NomesDependentes { get; set; }
        public List<string>? NomesDisciplinas { get; set; }
        public List<PessoaModel>? Dependentes { get; set; }

        // Listas para dropdowns (Create/Edit)
        public SelectList? Cidades { get; set; }
        public SelectList? Disciplinas { get; set; }
        public SelectList? Responsaveis { get; set; }
    }
}