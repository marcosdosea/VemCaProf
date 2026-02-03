using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public class PessoaDTO
{
    public int Id { get; set; }
    public string? IdUsuario { get; set; }
    //Caracteristicas básicas para criação de conta
    public string Nome { get; set; } = null!;
    public string Sobrenome { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string Genero { get; set; } = null!;
    public DateTime DataNascimento { get; set; }
    
    //Dados de endereço para Criacão de conta
    public string Cep { get; set; } = null!;
    public string Rua { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string Complemento { get; set; } = null!;
    public string Bairro { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
}