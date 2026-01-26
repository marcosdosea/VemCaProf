using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public class PessoaDTO
{
    public int Id { get; set; }
    //Caracteristicas básicas para criação de conta
    [Required (ErrorMessage = "O nome é obrigatório.")]
    public string Nome { get; set; } = null!;
    [Required (ErrorMessage = "O sobrenome é obrigatório.")]
    public string Sobrenome { get; set; } = null!;
    [Required (ErrorMessage = "O CPF é obrigatório.")]
    public string Cpf { get; set; } = null!;
    [Required (ErrorMessage = "O email é obrigatório.")]
    public string Email { get; set; } = null!;
    [Required (ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = null!;
    [Required (ErrorMessage = "O telefone é obrigatório.")]
    public string Telefone { get; set; } = null!;
    [Required (ErrorMessage = "O gênero é obrigatório.")]
    public string Genero { get; set; } = null!;
    [Required (ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime DataNascimento { get; set; }
    
    //Dados de endereço para Criacão de conta
    [Required (ErrorMessage = "O CEP é obrigatório.")]
    public string Cep { get; set; } = null!;
    [Required (ErrorMessage = "A rua é obrigatória.")]
    public string Rua { get; set; } = null!;
    [Required (ErrorMessage = "O número é obrigatório.")]
    public string Numero { get; set; } = null!;
    [Required (ErrorMessage = "O complemento é obrigatório.")]
    public string Complemento { get; set; } = null!;
    [Required (ErrorMessage = "O bairro é obrigatório.")]
    public string Bairro { get; set; } = null!;
    [Required (ErrorMessage = "A cidade é obrigatória.")]
    public string Cidade { get; set; } = null!;
    [Required (ErrorMessage = "O estado é obrigatório.")]
    public string Estado { get; set; } = null!;
}