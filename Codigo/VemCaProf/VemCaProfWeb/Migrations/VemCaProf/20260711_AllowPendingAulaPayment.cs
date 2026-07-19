using Core;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VemCaProfWeb.Migrations.VemCaProf
{
    [DbContext(typeof(VemCaProfContext))]
    [Migration("20260711_AllowPendingAulaPayment")]
    public partial class AllowPendingAulaPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE `VCP`.`Aula`
                    MODIFY COLUMN `dataHoraPagamento` DATETIME NULL,
                    MODIFY COLUMN `metodoPagamento` ENUM('P', 'C', 'D') NULL,
                    ADD COLUMN `idDisponibilidadeHorario` INT NULL,
                    ADD UNIQUE INDEX `uq_Aula_DisponibilidadeHorario` (`idDisponibilidadeHorario`),
                    ADD CONSTRAINT `fk_Aula_DisponibilidadeHorario`
                        FOREIGN KEY (`idDisponibilidadeHorario`)
                        REFERENCES `VCP`.`DisponibilidadeHorario` (`id`)
                        ON DELETE RESTRICT;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE `VCP`.`Aula`
                    DROP FOREIGN KEY `fk_Aula_DisponibilidadeHorario`,
                    DROP INDEX `uq_Aula_DisponibilidadeHorario`,
                    DROP COLUMN `idDisponibilidadeHorario`,
                    MODIFY COLUMN `metodoPagamento` ENUM('P', 'C', 'D') NOT NULL,
                    MODIFY COLUMN `dataHoraPagamento` DATETIME NOT NULL;
                """);
        }
    }
}
