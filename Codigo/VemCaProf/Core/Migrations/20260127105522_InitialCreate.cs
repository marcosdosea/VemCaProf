using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "VCP");

            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cidade",
                schema: "VCP",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    estado = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Disciplina",
                schema: "VCP",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    descricao = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    nivel = table.Column<string>(type: "enum('F1','F2','M1')", nullable: true, comment: "F1 = Ensino Fundamental Menor\nF2 = Ensino Fundamental Maior\nM1 = Ensino Medio ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pessoa",
                schema: "VCP",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    sobrenome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    cpf = table.Column<string>(type: "char(11)", fixedLength: true, maxLength: 11, nullable: false),
                    email = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    telefone = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    senha = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    genero = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    dataNascimento = table.Column<DateTime>(type: "date", nullable: false),
                    cep = table.Column<string>(type: "char(8)", fixedLength: true, maxLength: 8, nullable: false),
                    rua = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    numero = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    complemento = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    bairro = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    cidade = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    estado = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    quantidadeDeDependentes = table.Column<int>(type: "int", nullable: true),
                    alunoDeMenor = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    descricaoProfessor = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    libras = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    atipico = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    diploma = table.Column<byte[]>(type: "blob", nullable: true),
                    fotoDocumento = table.Column<byte[]>(type: "blob", nullable: true),
                    fotoPerfil = table.Column<byte[]>(type: "blob", nullable: true),
                    idCidade = table.Column<int>(type: "int", nullable: true),
                    responsavelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_Pessoa_Responsavel",
                        column: x => x.responsavelId,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Pessoa_Cidade1",
                        column: x => x.idCidade,
                        principalSchema: "VCP",
                        principalTable: "Cidade",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Aula",
                schema: "VCP",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    dataHorarioInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataHorarioFinal = table.Column<DateTime>(type: "datetime", nullable: false),
                    descricao = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    dataHoraPagamento = table.Column<DateTime>(type: "datetime", nullable: false),
                    valor = table.Column<double>(type: "double", nullable: false),
                    metodoPagamento = table.Column<string>(type: "enum('P','C','D')", nullable: false, comment: "P = Pix\nC = Credito\nD = Debito"),
                    status = table.Column<string>(type: "enum('AG','RE','PG','AP')", nullable: false, comment: "AG = Agendada\nRE = Realizada\nPG = Paga\nAP = Aguardando Pagamento\n"),
                    idDisciplina = table.Column<int>(type: "int", nullable: false),
                    idResponsavel = table.Column<int>(type: "int", nullable: false),
                    idAluno = table.Column<int>(type: "int", nullable: false),
                    idProfessor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Aula_Disciplina1",
                        column: x => x.idDisciplina,
                        principalSchema: "VCP",
                        principalTable: "Disciplina",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Aula_Pessoa1",
                        column: x => x.idResponsavel,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Aula_Pessoa2",
                        column: x => x.idProfessor,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Aula_Pessoa3",
                        column: x => x.idAluno,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Disciplina_Professor",
                schema: "VCP",
                columns: table => new
                {
                    idProfessor = table.Column<int>(type: "int", nullable: false),
                    idDisciplina = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idProfessor, x.idDisciplina });
                    table.ForeignKey(
                        name: "fk_Pessoa_has_Disciplina_Disciplina1",
                        column: x => x.idDisciplina,
                        principalSchema: "VCP",
                        principalTable: "Disciplina",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Pessoa_has_Disciplina_Pessoa1",
                        column: x => x.idProfessor,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DisponibilidadeHorario",
                schema: "VCP",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    dia = table.Column<DateTime>(type: "date", nullable: false),
                    horarioInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    horarioFim = table.Column<TimeSpan>(type: "time", nullable: false),
                    idProfessor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_DisponibilidadeHorario_Pessoa1",
                        column: x => x.idProfessor,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Penalidade",
                schema: "VCP",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    dataHorarioInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    descricao = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    dataHoraFim = table.Column<DateTime>(type: "datetime", nullable: true),
                    tipo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    idProfessor = table.Column<int>(type: "int", nullable: false),
                    idResponsavel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Penalidade_Pessoa1",
                        column: x => x.idProfessor,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Penalidade_Pessoa2",
                        column: x => x.idResponsavel,
                        principalSchema: "VCP",
                        principalTable: "Pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "fk_Aula_Disciplina1_idx",
                schema: "VCP",
                table: "Aula",
                column: "idDisciplina");

            migrationBuilder.CreateIndex(
                name: "fk_Aula_Pessoa1_idx",
                schema: "VCP",
                table: "Aula",
                column: "idResponsavel");

            migrationBuilder.CreateIndex(
                name: "fk_Aula_Pessoa2_idx",
                schema: "VCP",
                table: "Aula",
                column: "idProfessor");

            migrationBuilder.CreateIndex(
                name: "fk_Aula_Pessoa3_idx",
                schema: "VCP",
                table: "Aula",
                column: "idAluno");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                schema: "VCP",
                table: "Cidade",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE1",
                schema: "VCP",
                table: "Disciplina",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_has_Disciplina_Disciplina1_idx",
                schema: "VCP",
                table: "Disciplina_Professor",
                column: "idDisciplina");

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_has_Disciplina_Pessoa1_idx",
                schema: "VCP",
                table: "Disciplina_Professor",
                column: "idProfessor");

            migrationBuilder.CreateIndex(
                name: "fk_DisponibilidadeHorario_Pessoa1_idx",
                schema: "VCP",
                table: "DisponibilidadeHorario",
                column: "idProfessor");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE2",
                schema: "VCP",
                table: "DisponibilidadeHorario",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Penalidade_Pessoa1_idx",
                schema: "VCP",
                table: "Penalidade",
                column: "idProfessor");

            migrationBuilder.CreateIndex(
                name: "fk_Penalidade_Pessoa2_idx",
                schema: "VCP",
                table: "Penalidade",
                column: "idResponsavel");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE3",
                schema: "VCP",
                table: "Penalidade",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "cpf_UNIQUE",
                schema: "VCP",
                table: "Pessoa",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_Cidade1_idx",
                schema: "VCP",
                table: "Pessoa",
                column: "idCidade");

            migrationBuilder.CreateIndex(
                name: "FK_Pessoa_Responsavel",
                schema: "VCP",
                table: "Pessoa",
                column: "responsavelId");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE4",
                schema: "VCP",
                table: "Pessoa",
                column: "id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aula",
                schema: "VCP");

            migrationBuilder.DropTable(
                name: "Disciplina_Professor",
                schema: "VCP");

            migrationBuilder.DropTable(
                name: "DisponibilidadeHorario",
                schema: "VCP");

            migrationBuilder.DropTable(
                name: "Penalidade",
                schema: "VCP");

            migrationBuilder.DropTable(
                name: "Disciplina",
                schema: "VCP");

            migrationBuilder.DropTable(
                name: "Pessoa",
                schema: "VCP");

            migrationBuilder.DropTable(
                name: "Cidade",
                schema: "VCP");
        }
    }
}
