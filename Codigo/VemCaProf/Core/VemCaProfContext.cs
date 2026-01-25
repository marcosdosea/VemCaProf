using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class VemCaProfContext : DbContext
{
    public VemCaProfContext()
    {
    }

    public VemCaProfContext(DbContextOptions<VemCaProfContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aula> Aulas { get; set; }

    public virtual DbSet<Cidade> Cidades { get; set; }

    public virtual DbSet<Disciplina> Disciplinas { get; set; }

    public virtual DbSet<Pagamento> Pagamentos { get; set; }

    public virtual DbSet<Penalidade> Penalidades { get; set; }

    public virtual DbSet<Pessoa> Pessoas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aula");

            entity.HasIndex(e => e.IdDisciplinaAula, "fk_Aula_Disciplina1_idx");

            entity.HasIndex(e => e.IdAulaPagamento, "fk_Aula_Pagamento1_idx");

            entity.HasIndex(e => e.IdResponsavelAula, "fk_Aula_Pessoa1_idx");

            entity.HasIndex(e => e.IdProfessorAula, "fk_Aula_Pessoa2_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataHorarioFinal)
                .HasColumnType("datetime")
                .HasColumnName("dataHorarioFinal");
            entity.Property(e => e.DataHorarioInicio)
                .HasColumnType("datetime")
                .HasColumnName("dataHorarioInicio");
            entity.Property(e => e.Descricao)
                .HasMaxLength(45)
                .HasColumnName("descricao");
            entity.Property(e => e.IdAulaPagamento).HasColumnName("idAulaPagamento");
            entity.Property(e => e.IdDisciplinaAula).HasColumnName("idDisciplinaAula");
            entity.Property(e => e.IdProfessorAula).HasColumnName("idProfessorAula");
            entity.Property(e => e.IdResponsavelAula).HasColumnName("idResponsavelAula");
        });

        modelBuilder.Entity<Cidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cidade");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasColumnName("estado");
            entity.Property(e => e.Nome)
                .HasMaxLength(45)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Disciplina>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("disciplina");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .HasColumnName("descricao");
            entity.Property(e => e.Nivel)
                .HasComment("F1 = Ensino Fundamental Menor\nF2 = Ensino Fundamental Maior\nM1 = Ensino Medio ")
                .HasColumnType("enum('F1','F2','M1')")
                .HasColumnName("nivel");
            entity.Property(e => e.Nome)
                .HasMaxLength(45)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pagamento");

            entity.HasIndex(e => e.IdResponsavelPagamento, "fk_Pagamento_Pessoa1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataHora)
                .HasColumnType("datetime")
                .HasColumnName("dataHora");
            entity.Property(e => e.IdResponsavelPagamento).HasColumnName("idResponsavelPagamento");
            entity.Property(e => e.Status)
                .HasMaxLength(45)
                .HasColumnName("status");
            entity.Property(e => e.Valor).HasColumnName("valor");
        });

        modelBuilder.Entity<Penalidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("penalidade");

            entity.HasIndex(e => e.IdProfessor, "fk_Penalidade_Pessoa1_idx");

            entity.HasIndex(e => e.IdResponsavel, "fk_Penalidade_Pessoa2_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataHoraFim)
                .HasColumnType("datetime")
                .HasColumnName("dataHoraFim");
            entity.Property(e => e.DataHorarioInicio)
                .HasColumnType("datetime")
                .HasColumnName("dataHorarioInicio");
            entity.Property(e => e.Descricao)
                .HasMaxLength(45)
                .HasColumnName("descricao");
            entity.Property(e => e.IdProfessor).HasColumnName("idProfessor");
            entity.Property(e => e.IdResponsavel).HasColumnName("idResponsavel");
            entity.Property(e => e.Tipo)
                .HasMaxLength(45)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pessoa");

            entity.HasIndex(e => e.Cpf, "cpf_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdCidade, "fk_Pessoa_Cidade1_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlunoDeMenor).HasColumnName("alunoDeMenor");
            entity.Property(e => e.Atipico).HasColumnName("atipico");
            entity.Property(e => e.Bairro)
                .HasMaxLength(45)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .IsFixedLength()
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(45)
                .HasColumnName("cidade");
            entity.Property(e => e.Complemento)
                .HasMaxLength(45)
                .HasColumnName("complemento");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpf");
            entity.Property(e => e.DataNascimento)
                .HasColumnType("date")
                .HasColumnName("dataNascimento");
            entity.Property(e => e.DescricaoProfessor)
                .HasMaxLength(45)
                .HasColumnName("descricaoProfessor");
            entity.Property(e => e.Diploma)
                .HasColumnType("blob")
                .HasColumnName("diploma");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(45)
                .HasColumnName("estado");
            entity.Property(e => e.FotoDocumento)
                .HasColumnType("blob")
                .HasColumnName("fotoDocumento");
            entity.Property(e => e.FotoPerfil)
                .HasColumnType("blob")
                .HasColumnName("fotoPerfil");
            entity.Property(e => e.Genero)
                .HasMaxLength(45)
                .HasColumnName("genero");
            entity.Property(e => e.IdCidade).HasColumnName("idCidade");
            entity.Property(e => e.Libras).HasColumnName("libras");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .HasColumnName("nome");
            entity.Property(e => e.Numero)
                .HasMaxLength(45)
                .HasColumnName("numero");
            entity.Property(e => e.QuantidadeDeDependentes).HasColumnName("quantidadeDeDependentes");
            entity.Property(e => e.Rua)
                .HasMaxLength(45)
                .HasColumnName("rua");
            entity.Property(e => e.Senha)
                .HasMaxLength(45)
                .HasColumnName("senha");
            entity.Property(e => e.Sobrenome)
                .HasMaxLength(50)
                .HasColumnName("sobrenome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(45)
                .HasColumnName("telefone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
