using System;
using System.Collections.Generic;

namespace Core;

public partial class Pessoa
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Sobrenome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public string Genero { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    public string Cep { get; set; } = null!;

    public string Rua { get; set; } = null!;

    public string Numero { get; set; } = null!;

    public string Complemento { get; set; } = null!;

    public string Bairro { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public int? QuantidadeDeDependentes { get; set; }

    public bool? AlunoDeMenor { get; set; }

    public string? DescricaoProfessor { get; set; }

    public bool? Libras { get; set; }

    public bool? Atipico { get; set; }

    public byte[]? Diploma { get; set; }

    public byte[]? FotoDocumento { get; set; }

    public byte[]? FotoPerfil { get; set; }

    public int IdCidade { get; set; }
}
