using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VemCaProfWeb.Migrations.VemCaProf
{
    public partial class RecurringAvailability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Habilita disponibilidade recorrente — índice composto por horário + data
            // em vez de índice único apenas por horário.
            migrationBuilder.Sql("""
                ALTER TABLE `VCP`.`Aula`
                    DROP INDEX `uq_Aula_DisponibilidadeHorario`,
                    ADD UNIQUE INDEX `uq_Aula_DisponibilidadeHorario_Data`
                        (`idDisponibilidadeHorario`, `dataHorarioInicio`);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE `VCP`.`Aula`
                    DROP INDEX `uq_Aula_DisponibilidadeHorario_Data`,
                    ADD UNIQUE INDEX `uq_Aula_DisponibilidadeHorario` (`idDisponibilidadeHorario`);
                """);
        }
    }
}
