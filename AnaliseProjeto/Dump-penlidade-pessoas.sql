USE vcp;

-- Versão segura: INSERT IGNORE para evitar erros de duplicidade

INSERT IGNORE INTO vcp.pessoa(`id`,`nome`,`sobrenome`,`cpf`,`email`,`telefone`,`genero`,`dataNascimento`,`cep`,`rua`,`numero`,
`complemento`,`bairro`,`cidade`,`estado`,`quantidadeDeDependentes`,`alunoDeMenor`,`descricaoProfessor`,`libras`,`atipico`,
`diploma`,`fotoDocumento`,`fotoPerfil`,`idCidade`,`responsavelId`,`tipoPessoa`)
VALUES
(1,'Maria','Silva','12345678901','maria.silva@example.com','(79)99999-8888','Feminino','1990-05-20','49500000','Rua das Flores',
'123','Apto 201','Centro','Itabaiana','SE',2,0,'Professora de Matemática',1,0,1,NULL,NULL,101,1,'P'),
(2,'João','Pereira','98765432100','joao.pereira@example.com','(79)98888-7777','Masculino','1985-03-15','49500-01','Avenida Brasil',
'456','Casa','São José','Itabaiana','SE',1,0,'Professor de História',0,0,1,NULL,NULL,101,1,'P'),
(3,'Ana','Costa','11122233344','ana.costa@example.com','(79)97777-6666','Feminino','1992-11-10','49500-02','Rua da Educação',
'789','Bloco B','Serrano','Itabaiana','SE',0,0,'Professora de Biologia',1,1,1,NULL,NULL,101,1,'P'),
(4,'Carlos','Souza','22233344455','carlos.souza@example.com','(79)96666-5555','Masculino','1975-08-12','49500010','Rua do Comércio',
'321','Casa','Centro','Itabaiana','SE',3,0,NULL,0,0,0,NULL,NULL,101,2,'R'),

(5,'Fernanda','Oliveira','33344455566','fernanda.oliveira@example.com','(79)95555-4444','Feminino','1980-02-25','49500020','Avenida Sergipe',
'654','Bloco A','Serrano','Itabaiana','SE',2,0,NULL,0,0,0,NULL,NULL,101,2,'R'),

(6,'Roberto','Lima','44455566677','roberto.lima@example.com','(79)94444-3333','Masculino','1983-07-30','4950030','Rua das Palmeiras',
'987','Apto 101','São José','Itabaiana','SE',1,0,NULL,0,0,0,NULL,NULL,101,2,'R');



INSERT INTO `vcp`.`penalidade`(`id`, `dataHorarioInicio`, `descricao`, `dataHoraFim`, `tipo`, `idProfessor`, `idResponsavel`)
VALUES
(1, '2026-05-20 08:00:00', 'Atraso na entrega de atividade', '2026-05-20 09:00:00', 'Advertência', 1, 4),
(2, '2026-05-21 10:30:00', 'Uso de celular em sala de aula', '2026-05-21 11:00:00', 'Suspensão curta', 2, 6),
(3, '2026-05-22 14:00:00', 'Falta não justificada', '2026-05-22 15:00:00', 'Advertência', 3, 6);

-- Conferir os registros inseridos
SELECT * FROM vcp.penalidade;



