-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema IdentityUsers
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `IdentityUsers` ;

-- -----------------------------------------------------
-- Schema IdentityUsers
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `IdentityUsers` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
-- -----------------------------------------------------
-- Schema VCP
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `VCP` ;

-- -----------------------------------------------------
-- Schema VCP
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `VCP` DEFAULT CHARACTER SET utf8mb3 ;
USE `IdentityUsers` ;

-- -----------------------------------------------------
-- Table `IdentityUsers`.`AspNetRoles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`AspNetRoles` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`AspNetRoles` (
  `Id` VARCHAR(255) NOT NULL,
  `Name` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedName` VARCHAR(256) NULL DEFAULT NULL,
  `ConcurrencyStamp` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

CREATE UNIQUE INDEX `RoleNameIndex` ON `IdentityUsers`.`AspNetRoles` (`NormalizedName` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `IdentityUsers`.`AspNetRoleClaims`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`AspNetRoleClaims` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`AspNetRoleClaims` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `RoleId` VARCHAR(255) NOT NULL,
  `ClaimType` LONGTEXT NULL DEFAULT NULL,
  `ClaimValue` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `IdentityUsers`.`AspNetRoleClaims` (`RoleId` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `IdentityUsers`.`AspNetUsers`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`AspNetUsers` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`AspNetUsers` (
  `Id` VARCHAR(255) NOT NULL,
  `UserName` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedUserName` VARCHAR(256) NULL DEFAULT NULL,
  `Email` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedEmail` VARCHAR(256) NULL DEFAULT NULL,
  `EmailConfirmed` TINYINT(1) NOT NULL,
  `PasswordHash` LONGTEXT NULL DEFAULT NULL,
  `SecurityStamp` LONGTEXT NULL DEFAULT NULL,
  `ConcurrencyStamp` LONGTEXT NULL DEFAULT NULL,
  `PhoneNumber` LONGTEXT NULL DEFAULT NULL,
  `PhoneNumberConfirmed` TINYINT(1) NOT NULL,
  `TwoFactorEnabled` TINYINT(1) NOT NULL,
  `LockoutEnd` DATETIME NULL DEFAULT NULL,
  `LockoutEnabled` TINYINT(1) NOT NULL,
  `AccessFailedCount` INT NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

CREATE UNIQUE INDEX `UserNameIndex` ON `IdentityUsers`.`AspNetUsers` (`NormalizedUserName` ASC) VISIBLE;

CREATE INDEX `EmailIndex` ON `IdentityUsers`.`AspNetUsers` (`NormalizedEmail` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `IdentityUsers`.`AspNetUserClaims`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`AspNetUserClaims` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`AspNetUserClaims` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `UserId` VARCHAR(255) NOT NULL,
  `ClaimType` LONGTEXT NULL DEFAULT NULL,
  `ClaimValue` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `IdentityUsers`.`AspNetUserClaims` (`UserId` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `IdentityUsers`.`AspNetUserLogins`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`AspNetUserLogins` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`AspNetUserLogins` (
  `LoginProvider` VARCHAR(128) NOT NULL,
  `ProviderKey` VARCHAR(128) NOT NULL,
  `ProviderDisplayName` LONGTEXT NULL DEFAULT NULL,
  `UserId` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `IdentityUsers`.`AspNetUserLogins` (`UserId` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `IdentityUsers`.`AspNetUserRoles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`AspNetUserRoles` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`AspNetUserRoles` (
  `UserId` VARCHAR(255) NOT NULL,
  `RoleId` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `IdentityUsers`.`AspNetUserRoles` (`RoleId` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `IdentityUsers`.`AspNetUserTokens`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`AspNetUserTokens` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`AspNetUserTokens` (
  `UserId` VARCHAR(255) NOT NULL,
  `LoginProvider` VARCHAR(128) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `Value` LONGTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`UserId`, `LoginProvider`, `Name`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `IdentityUsers`.`__EFMigrationsHistory`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `IdentityUsers`.`__EFMigrationsHistory` ;

CREATE TABLE IF NOT EXISTS `IdentityUsers`.`__EFMigrationsHistory` (
  `MigrationId` VARCHAR(150) NOT NULL,
  `ProductVersion` VARCHAR(32) NOT NULL,
  PRIMARY KEY (`MigrationId`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

USE `VCP` ;

-- -----------------------------------------------------
-- Table `VCP`.`Disciplina`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`Disciplina` ;

CREATE TABLE IF NOT EXISTS `VCP`.`Disciplina` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(45) NOT NULL,
  `descricao` VARCHAR(200) NULL DEFAULT NULL,
  `nivel` ENUM('F1', 'F2', 'M1') NULL DEFAULT NULL COMMENT 'F1 = Ensino Fundamental Menor\\nF2 = Ensino Fundamental Maior\\nM1 = Ensino Medio ',
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 6
DEFAULT CHARACTER SET = utf8mb3;

CREATE UNIQUE INDEX `id_UNIQUE` ON `VCP`.`Disciplina` (`id` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `VCP`.`Cidade`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`Cidade` ;

CREATE TABLE IF NOT EXISTS `VCP`.`Cidade` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(45) NOT NULL,
  `estado` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 8
DEFAULT CHARACTER SET = utf8mb3;

CREATE UNIQUE INDEX `id_UNIQUE` ON `VCP`.`Cidade` (`id` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `VCP`.`Pessoa`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`Pessoa` ;

CREATE TABLE IF NOT EXISTS `VCP`.`Pessoa` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(50) NOT NULL,
  `sobrenome` VARCHAR(50) NOT NULL,
  `cpf` CHAR(11) NOT NULL,
  `email` VARCHAR(45) NOT NULL,
  `telefone` VARCHAR(45) NOT NULL,
  `genero` VARCHAR(45) NOT NULL,
  `dataNascimento` DATE NOT NULL,
  `cep` CHAR(8) NOT NULL,
  `rua` VARCHAR(45) NOT NULL,
  `numero` VARCHAR(45) NOT NULL,
  `complemento` VARCHAR(45) NOT NULL,
  `bairro` VARCHAR(45) NOT NULL,
  `cidade` VARCHAR(45) NOT NULL,
  `estado` VARCHAR(45) NOT NULL,
  `quantidadeDeDependentes` INT NULL DEFAULT NULL,
  `alunoDeMenor` TINYINT(1) NULL DEFAULT NULL,
  `descricaoProfessor` VARCHAR(45) NULL DEFAULT NULL,
  `libras` TINYINT(1) NULL DEFAULT NULL,
  `atipico` TINYINT(1) NULL DEFAULT NULL,
  `diploma` BLOB NULL DEFAULT NULL,
  `fotoDocumento` BLOB NULL DEFAULT NULL,
  `fotoPerfil` BLOB NULL DEFAULT NULL,
  `idCidade` INT NULL DEFAULT NULL,
  `responsavelId` INT NULL DEFAULT NULL,
  `tipoPessoa` ENUM('R', 'A', 'P') NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 22
DEFAULT CHARACTER SET = utf8mb3;

CREATE UNIQUE INDEX `cpf_UNIQUE` ON `VCP`.`Pessoa` (`cpf` ASC) VISIBLE;

CREATE UNIQUE INDEX `id_UNIQUE` ON `VCP`.`Pessoa` (`id` ASC) VISIBLE;

CREATE INDEX `fk_Pessoa_Cidade1_idx` ON `VCP`.`Pessoa` (`idCidade` ASC) VISIBLE;

CREATE INDEX `FK_Pessoa_Responsavel_idx` ON `VCP`.`Pessoa` (`responsavelId` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `VCP`.`Aula`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`Aula` ;

CREATE TABLE IF NOT EXISTS `VCP`.`Aula` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `dataHorarioInicio` DATETIME NOT NULL,
  `dataHorarioFinal` DATETIME NOT NULL,
  `descricao` VARCHAR(45) NOT NULL,
  `dataHoraPagamento` DATETIME NOT NULL,
  `valor` DOUBLE NOT NULL,
  `metodoPagamento` ENUM('P', 'C', 'D') NOT NULL COMMENT 'P = Pix\\nC = Credito\\nD = Debito',
  `status` ENUM('AG', 'RE', 'PG', 'AP', 'CA', 'CO') NOT NULL COMMENT 'AG = Agendada\\nRE = Realizada\\nPG = Paga\\nAP = Aguardando Pagamento\\n\nCA = Cancelada\nCO = Confirmada',
  `idDisciplina` INT UNSIGNED NOT NULL,
  `idResponsavel` INT NOT NULL,
  `idAluno` INT NOT NULL,
  `idProfessor` INT NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE INDEX `fk_Aula_Disciplina1_idx` ON `VCP`.`Aula` (`idDisciplina` ASC) VISIBLE;

CREATE INDEX `fk_Aula_Pessoa1_idx` ON `VCP`.`Aula` (`idResponsavel` ASC) VISIBLE;

CREATE INDEX `fk_Aula_Pessoa3_idx` ON `VCP`.`Aula` (`idAluno` ASC) VISIBLE;

CREATE INDEX `fk_Aula_Pessoa2_idx` ON `VCP`.`Aula` (`idProfessor` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `VCP`.`Disciplina_Professor`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`Disciplina_Professor` ;

CREATE TABLE IF NOT EXISTS `VCP`.`Disciplina_Professor` (
  `idProfessor` INT NOT NULL,
  `idDisciplina` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`idProfessor`, `idDisciplina`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE INDEX `fk_Pessoa_has_Disciplina_Disciplina1_idx` ON `VCP`.`Disciplina_Professor` (`idDisciplina` ASC) VISIBLE;

CREATE INDEX `fk_Pessoa_has_Disciplina_Pessoa1_idx` ON `VCP`.`Disciplina_Professor` (`idProfessor` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `VCP`.`DisponibilidadeHorario`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`DisponibilidadeHorario` ;

CREATE TABLE IF NOT EXISTS `VCP`.`DisponibilidadeHorario` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `dia` DATE NOT NULL,
  `horarioInicio` TIME NOT NULL,
  `horarioFim` TIME NOT NULL,
  `idProfessor` INT NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;

CREATE UNIQUE INDEX `id_UNIQUE` ON `VCP`.`DisponibilidadeHorario` (`id` ASC) VISIBLE;

CREATE INDEX `fk_DisponibilidadeHorario_Pessoa1_idx` ON `VCP`.`DisponibilidadeHorario` (`idProfessor` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `VCP`.`Penalidade`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`Penalidade` ;

CREATE TABLE IF NOT EXISTS `VCP`.`Penalidade` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `dataHorarioInicio` DATETIME NOT NULL,
  `descricao` VARCHAR(45) NOT NULL,
  `dataHoraFim` DATETIME NULL DEFAULT NULL,
  `tipo` VARCHAR(45) NULL DEFAULT NULL,
  `idProfessor` INT NOT NULL,
  `idResponsavel` INT NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb3;

CREATE UNIQUE INDEX `id_UNIQUE` ON `VCP`.`Penalidade` (`id` ASC) VISIBLE;

CREATE INDEX `fk_Penalidade_Pessoa1_idx` ON `VCP`.`Penalidade` (`idProfessor` ASC) VISIBLE;

CREATE INDEX `fk_Penalidade_Pessoa2_idx` ON `VCP`.`Penalidade` (`idResponsavel` ASC) VISIBLE;


-- -----------------------------------------------------
-- Table `VCP`.`__EFMigrationsHistory`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `VCP`.`__EFMigrationsHistory` ;

CREATE TABLE IF NOT EXISTS `VCP`.`__EFMigrationsHistory` (
  `MigrationId` VARCHAR(150) NOT NULL,
  `ProductVersion` VARCHAR(32) NOT NULL,
  PRIMARY KEY (`MigrationId`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
