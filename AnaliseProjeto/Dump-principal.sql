
INSERT INTO `identityusers`.`aspnetroles`
(`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`)
VALUES
('1', 'Professor', 'Professor', null),
('2', 'Aluno', 'Aluno', null),
('3', 'Responsavel', 'Responsavel', null),
('4', 'Admin', 'Admin', null);

SELECT * FROM identityusers.aspnetroles;

-- ADMINISTRADORES
INSERT INTO `vcp`.`pessoa`
(`id`,`nome`,`sobrenome`,`cpf`,`email`,`telefone`,`genero`,
 `dataNascimento`,`cep`,`rua`,`numero`,`complemento`,`bairro`,
 `cidade`,`estado`,`quantidadeDeDependentes`,`alunoDeMenor`,
 `descricaoProfessor`,`libras`,`atipico`,`diploma`,
 `fotoDocumento`,`fotoPerfil`,`idCidade`,`responsavelId`,`tipoPessoa`)
VALUES
('1','Carlos','Admin','11111111111','admin1@email.com','999999999','M',
 '1980-01-01','49000000','Rua A','10','','Centro','Itabaiana','SE',
 0,0,NULL,0,0,NULL,NULL,NULL,1,NULL,'A'),
('2','Maria','Admin','22222222222','admin2@email.com','888888888','F',
 '1985-02-02','49000000','Rua B','20','','Centro','Itabaiana','SE',
 0,0,NULL,0,0,NULL,NULL,NULL,1,NULL,'A');

INSERT INTO `identityusers`.`aspnetusers`
(`Id`,`UserName`,`NormalizedUserName`,`Email`,`NormalizedEmail`,
 `EmailConfirmed`,`PasswordHash`,`SecurityStamp`,`ConcurrencyStamp`,
 `PhoneNumber`,`PhoneNumberConfirmed`,`TwoFactorEnabled`,
 `LockoutEnd`,`LockoutEnabled`,`AccessFailedCount`)
VALUES
(UUID(),'admin1','ADMIN1','admin1@email.com','ADMIN1@EMAIL.COM',
 1,'hash','secstamp','concstamp','999999999',1,0,NULL,1,0),
(UUID(),'admin2','ADMIN2','admin2@email.com','ADMIN2@EMAIL.COM',
 1,'hash','secstamp','concstamp','888888888',1,0,NULL,1,0);


-- RESPONSÁVEL
INSERT INTO `vcp`.`pessoa`
VALUES
('3','João','Responsável','33333333333','resp@email.com','777777777','M',
 '1975-03-03','49000000','Rua C','30','','Centro','Itabaiana','SE',
 2,0,NULL,0,0,NULL,NULL,NULL,1,NULL,'R');

INSERT INTO `identityusers`.`aspnetusers`
VALUES
(UUID(),'responsavel1','RESPONSAVEL1','resp@email.com','RESP@EMAIL.COM',
 1,'hash','secstamp','concstamp','777777777',1,0,NULL,1,0);



-- ALUNOS vinculados ao RESPONSÁVEL
SET @respId = (SELECT id FROM `vcp`.`pessoa` WHERE email='resp@email.com');
INSERT INTO `vcp`.`pessoa`
VALUES
('4','Pedro','Aluno','44444444444','aluno1@email.com','666666666','M',
 '2010-04-04','49000000','Rua D','40','','Centro','Itabaiana','SE',
 0,1,NULL,0,0,NULL,NULL,NULL,1, @respId,'A'),
('5','Ana','Aluno','55555555555','aluno2@email.com','555555555','F',
 '2012-05-05','49000000','Rua E','50','','Centro','Itabaiana','SE',
 0,1,NULL,0,0,NULL,NULL,NULL,1, @respId,'A');
 
INSERT INTO `identityusers`.`aspnetusers`
VALUES
(UUID(),'aluno1','ALUNO1','aluno1@email.com','ALUNO1@EMAIL.COM',
 1,'hash','secstamp','concstamp','666666666',1,0,NULL,1,0),
(UUID(),'aluno2','ALUNO2','aluno2@email.com','ALUNO2@EMAIL.COM',
 1,'hash','secstamp','concstamp','555555555',1,0,NULL,1,0);



-- ALUNO independente
INSERT INTO `vcp`.`pessoa`
VALUES
('6','Lucas','Aluno','66666666666','aluno3@email.com','444444444','M',
 '2008-06-06','49000000','Rua F','60','','Centro','Itabaiana','SE',
 0,0,NULL,0,0,NULL,NULL,NULL,1,NULL,'A');

INSERT INTO `identityusers`.`aspnetusers`
VALUES
(UUID(),'aluno3','ALUNO3','aluno3@email.com','ALUNO3@EMAIL.COM',
 1,'hash','secstamp','concstamp','444444444',1,0,NULL,1,0);



-- PROFESSORES
INSERT INTO `vcp`.`pessoa`
VALUES
('7','Paulo','Professor','77777777777','prof1@email.com','333333333','M',
 '1980-07-07','49000000','Rua G','70','','Centro','Itabaiana','SE',
 0,0,'Matemática',0,0,'Licenciatura',NULL,NULL,1,NULL,'P'),
('8','Clara','Professor','88888888888','prof2@email.com','222222222','F',
 '1982-08-08','49000000','Rua H','80','','Centro','Itabaiana','SE',
 0,0,'Português',0,0,'Licenciatura',NULL,NULL,1,NULL,'P');
 
INSERT INTO `identityusers`.`aspnetusers`
VALUES
(UUID(),'prof1','PROF1','prof1@email.com','PROF1@EMAIL.COM',
 1,'hash','secstamp','concstamp','333333333',1,0,NULL,1,0),
(UUID(),'prof2','PROF2','prof2@email.com','PROF2@EMAIL.COM',
 1,'hash','secstamp','concstamp','222222222',1,0,NULL,1,0);

-- Cidades
INSERT INTO `vcp`.`cidade`
(`id`, `nome`, `estado`)
VALUES
('1', 'Itabaiana', 'SE'),
('2', 'Aracaju', 'SE'),
('3', 'São Paulo', 'SP'),
('4', 'Rio de Janeiro', 'RJ'),
('5', 'Salvador', 'BA'),
('6', 'Recife', 'PE'),
('7', 'Fortaleza', 'CE'),
('8', 'Belo Horizonte', 'MG');

-- Diciplinas
INSERT INTO `vcp`.`disciplina`
(`id`, `nome`, `descricao`, `nivel`)
VALUES
('1', 'Matemática', 'Disciplina de cálculos, álgebra e geometria', 'F1'),
('2', 'Português', 'Disciplina de leitura, escrita e gramática', 'F1'),
('3', 'História', 'Disciplina sobre fatos históricos e evolução da sociedade', 'M1'),
('4', 'Geografia', 'Disciplina sobre espaço geográfico, relevo e clima', 'M1'),
('5', 'Física', 'Disciplina sobre leis da natureza e fenômenos físicos', 'M1'),
('6', 'Química', 'Disciplina sobre substâncias, reações e experimentos', 'M1'),
('7', 'Biologia', 'Disciplina sobre seres vivos e ecossistemas', 'M1'),
('8', 'Inglês', 'Disciplina de língua estrangeira e comunicação', 'F2'),
('9', 'Educação Física', 'Disciplina de práticas esportivas e saúde', 'F2');


-- Professor 7 vinculado a várias disciplinas
INSERT INTO `vcp`.`disciplina_professor` (`idProfessor`, `idDisciplina`) VALUES
(7, 1), -- Matemática
(7, 2), -- Português
(7, 3), -- História
(7, 4), -- Geografia
(7, 5); -- Física

-- Professor 8 vinculado a várias disciplinas
INSERT INTO `vcp`.`disciplina_professor` (`idProfessor`, `idDisciplina`) VALUES
(8, 6), -- Química
(8, 7), -- Biologia
(8, 8), -- Inglês
(8, 9); -- Educação Física


-- Disponibilidade do Professor 7
INSERT INTO `vcp`.`disponibilidadehorario`
(`id`, `dia`, `horarioInicio`, `horarioFim`, `idProfessor`)
VALUES
('1', '2026-07-13', '08:00:00', '12:00:00', 7),-- Segunda
('2', '2026-07-15', '14:00:00', '18:00:00', 7),-- Quarta
('3', '2026-07-17', '08:00:00', '12:00:00', 7);-- Sexta

-- Disponibilidade do Professor 8
INSERT INTO `vcp`.`disponibilidadehorario`
(`id`, `dia`, `horarioInicio`, `horarioFim`, `idProfessor`)
VALUES
('4', '2026-07-14', '09:00:00', '13:00:00', 8),-- Terça
('5', '2026-07-16', '15:00:00', '19:00:00', 8), -- Quinta
('6', '2026-07-18', '08:00:00', '11:00:00', 8);-- Sábado


-- ================== AULAS ================================================
-- Aula de Matemática com Professor 7 e Aluno 1
SET @IdResp = (SELECT id FROM `vcp`.`pessoa` WHERE email='resp@email.com');
SET @IdAluno = (SELECT id FROM `vcp`.`pessoa` WHERE email='aluno1@email.com');
INSERT INTO `vcp`.`aula`
(`id`, `dataHorarioInicio`, `dataHorarioFinal`, `descricao`, `dataHoraPagamento`,
 `valor`, `metodoPagamento`, `status`, `idDisciplina`, `idResponsavel`, `idAluno`, `idProfessor`)
VALUES
('1', '2026-07-13 08:00:00', '2026-07-13 10:00:00', 'Aula de Matemática - Introdução à Álgebra',
 '2026-07-12 18:00:00', 100.00, 'P', 'AG', 1, 
 @IdResp,
 @IdAluno,
 7);

-- Aula de Português com Professor 7 e Aluno 2
-- SET @IdResp = (SELECT id FROM `vcp`.`pessoa` WHERE email='resp@email.com');
SET @IdAluno = (SELECT id FROM `vcp`.`pessoa` WHERE email='aluno2@email.com');
INSERT INTO `vcp`.`aula`
VALUES
('2', '2026-07-15 14:00:00', '2026-07-15 16:00:00', 'Aula de Português - Gramática básica',
 '2026-07-14 20:00:00', 90.00, 'C', 'AG', 2,
 @IdResp,
 @IdAluno,
 7);

-- Aula de Química com Professor 8 e Aluno 3 (independente)
SET @IdAluno = (SELECT id FROM `vcp`.`pessoa` WHERE email='aluno3@email.com');

INSERT INTO `vcp`.`aula`
VALUES
('3', '2026-07-16 09:00:00', '2026-07-16 11:00:00', 'Aula de Química - Reações químicas',
 '2026-07-15 19:00:00', 120.00, 'D', 'AG', 6,
 0, -- aluno independente, sem responsável
 @IdAluno,
 8);

-- Aula de Biologia com Professor 8 e Aluno 1
-- SET @IdResp = (SELECT id FROM `vcp`.`pessoa` WHERE email='resp@email.com');
SET @IdAluno = (SELECT id FROM `vcp`.`pessoa` WHERE email='aluno3@email.com');
INSERT INTO `vcp`.`aula`
VALUES
('4', '2026-07-18 08:00:00', '2026-07-18 10:00:00', 'Aula de Biologia - Ecossistemas',
 '2026-07-17 21:00:00', 110.00, 'P', 'PG', 7,
 @IdResp,
 @IdAluno,
 8);
