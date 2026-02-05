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

    public virtual DbSet<DisponibilidadeHorario> DisponibilidadeHorarios { get; set; }

    public virtual DbSet<Penalidade> Penalidades { get; set; }

    public virtual DbSet<Pessoa> Pessoas { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Aula", "VCP");

            entity.HasIndex(e => e.IdDisciplina, "fk_Aula_Disciplina1_idx");

            entity.HasIndex(e => e.IdResponsavel, "fk_Aula_Pessoa1_idx");

            entity.HasIndex(e => e.IdProfessor, "fk_Aula_Pessoa2_idx");

            entity.HasIndex(e => e.IdAluno, "fk_Aula_Pessoa3_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataHoraPagamento)
                .HasColumnType("datetime")
                .HasColumnName("dataHoraPagamento");
            entity.Property(e => e.DataHorarioFinal)
                .HasColumnType("datetime")
                .HasColumnName("dataHorarioFinal");
            entity.Property(e => e.DataHorarioInicio)
                .HasColumnType("datetime")
                .HasColumnName("dataHorarioInicio");
            entity.Property(e => e.Descricao)
                .HasMaxLength(45)
                .HasColumnName("descricao");
            entity.Property(e => e.IdAluno).HasColumnName("idAluno");
            entity.Property(e => e.IdDisciplina).HasColumnName("idDisciplina");
            entity.Property(e => e.IdProfessor).HasColumnName("idProfessor");
            entity.Property(e => e.IdResponsavel).HasColumnName("idResponsavel");
            entity.Property(e => e.MetodoPagamento)
                .HasComment("P = Pix\nC = Credito\nD = Debito")
                .HasColumnType("enum('P','C','D')")
                .HasColumnName("metodoPagamento");
            entity.Property(e => e.Status)
                .HasComment("AG = Agendada\nRE = Realizada\nPG = Paga\nAP = Aguardando Pagamento\nCA = Cancelada\nCO = Confirmada")
                .HasColumnType("enum('AG','RE','PG','AP','CA','CO')")
                .HasColumnName("status");
            entity.Property(e => e.Valor).HasColumnName("valor");

            entity.HasOne(d => d.IdAlunoNavigation).WithMany(p => p.AulaIdAlunoNavigations)
                .HasForeignKey(d => d.IdAluno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Aula_Pessoa3");

            entity.HasOne(d => d.IdDisciplinaNavigation).WithMany(p => p.Aulas)
                .HasForeignKey(d => d.IdDisciplina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Aula_Disciplina1");

            entity.HasOne(d => d.IdProfessorNavigation).WithMany(p => p.AulaIdProfessorNavigations)
                .HasForeignKey(d => d.IdProfessor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Aula_Pessoa2");

            entity.HasOne(d => d.IdResponsavelNavigation).WithMany(p => p.AulaIdResponsavelNavigations)
                .HasForeignKey(d => d.IdResponsavel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Aula_Pessoa1");
        });

        modelBuilder.Entity<Cidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Cidade", "VCP");

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

            entity.ToTable("Disciplina", "VCP");

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

        modelBuilder.Entity<DisponibilidadeHorario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("DisponibilidadeHorario", "VCP");

            entity.HasIndex(e => e.IdProfessor, "fk_DisponibilidadeHorario_Pessoa1_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dia)
                .HasColumnType("date")
                .HasColumnName("dia");
            entity.Property(e => e.HorarioFim)
                .HasColumnType("time")
                .HasColumnName("horarioFim");
            entity.Property(e => e.HorarioInicio)
                .HasColumnType("time")
                .HasColumnName("horarioInicio");
            entity.Property(e => e.IdProfessor).HasColumnName("idProfessor");

            entity.HasOne(d => d.IdProfessorNavigation).WithMany(p => p.DisponibilidadeHorarios)
                .HasForeignKey(d => d.IdProfessor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DisponibilidadeHorario_Pessoa1");
        });

        modelBuilder.Entity<Penalidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Penalidade", "VCP");

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

            entity.HasOne(d => d.IdProfessorNavigation).WithMany(p => p.PenalidadeIdProfessorNavigations)
                .HasForeignKey(d => d.IdProfessor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Penalidade_Pessoa1");

            entity.HasOne(d => d.IdResponsavelNavigation).WithMany(p => p.PenalidadeIdResponsavelNavigations)
                .HasForeignKey(d => d.IdResponsavel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Penalidade_Pessoa2");
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Pessoa", "VCP");

            entity.HasIndex(e => e.ResponsavelId, "FK_Pessoa_Responsavel");

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
            entity.Property(e => e.ResponsavelId).HasColumnName("responsavelId");
            entity.Property(e => e.Rua)
                .HasMaxLength(45)
                .HasColumnName("rua");
            entity.Property(e => e.Sobrenome)
                .HasMaxLength(50)
                .HasColumnName("sobrenome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(45)
                .HasColumnName("telefone");
            entity.Property(e => e.TipoPessoa)
                .HasComment("R = Responsável\nP = Professor\nA = Aluno")
                .HasColumnType("enum('R','P','A')")
                .HasColumnName("tipoPessoa");

            entity.HasOne(d => d.IdCidadeNavigation).WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdCidade)
                .HasConstraintName("fk_Pessoa_Cidade1");

            entity.HasOne(d => d.Responsavel).WithMany(p => p.InverseResponsavel)
                .HasForeignKey(d => d.ResponsavelId)
                .HasConstraintName("FK_Pessoa_Responsavel");

            entity.HasMany(d => d.IdDisciplinas).WithMany(p => p.IdProfessors)
                .UsingEntity<Dictionary<string, object>>(
                    "DisciplinaProfessor",
                    r => r.HasOne<Disciplina>().WithMany()
                        .HasForeignKey("IdDisciplina")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_Pessoa_has_Disciplina_Disciplina1"),
                    l => l.HasOne<Pessoa>().WithMany()
                        .HasForeignKey("IdProfessor")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_Pessoa_has_Disciplina_Pessoa1"),
                    j =>
                    {
                        j.HasKey("IdProfessor", "IdDisciplina").HasName("PRIMARY");
                        j.ToTable("Disciplina_Professor", "VCP");
                        j.HasIndex(new[] { "IdDisciplina" }, "fk_Pessoa_has_Disciplina_Disciplina1_idx");
                        j.HasIndex(new[] { "IdProfessor" }, "fk_Pessoa_has_Disciplina_Pessoa1_idx");
                        j.IndexerProperty<int>("IdProfessor").HasColumnName("idProfessor");
                        j.IndexerProperty<uint>("IdDisciplina").HasColumnName("idDisciplina");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
