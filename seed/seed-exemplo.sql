-- Dados de exemplo para o DoeSangue (idempotente: pode ser executado mais de uma vez).
-- Usa os hemocentros e o doador usuário que já existem no banco e cria:
--   * leads de exemplo para cada hemocentro
--   * agendamentos concluídos ao longo dos últimos ~70 dias, cada um com sua bolsa de sangue
--   * agendamentos cancelados e agendados (futuros e um já vencido, aguardando conclusão)
--   * depoimentos de exemplo

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRAN;

DECLARE @hoje datetime2 = CAST(CAST(GETDATE() AS date) AS datetime2);

IF NOT EXISTS (SELECT 1 FROM Hemocentros)
BEGIN
    PRINT N'Nenhum hemocentro cadastrado: cadastre ao menos um hemocentro antes de executar o seed.';
    ROLLBACK;
    RETURN;
END

SELECT ROW_NUMBER() OVER (ORDER BY NomeFantasia, Id) AS Idx,
       Id, NomeFantasia, UsuarioId,
       DATEPART(HOUR, AbreEm) AS HoraAbre,
       DATEPART(HOUR, FechaEm) AS HoraFecha
INTO #hemos
FROM Hemocentros;

DECLARE @qtdHemos int = (SELECT COUNT(*) FROM #hemos);

-- Leads de exemplo (4 por hemocentro; o índice do hemocentro é reaproveitado se houver menos de 2)
SELECT * INTO #leads FROM (VALUES
    (1, N'Mariana Souza',        '52998224725', '48991230001', CAST('1992-03-14' AS date), 'F', 'O+'),
    (1, N'Carlos Eduardo Lima',  '11144477735', '48991230002', CAST('1985-07-02' AS date), 'M', 'A+'),
    (1, N'Fernanda Alves',       '39053344705', '48991230003', CAST('1998-11-23' AS date), 'F', 'B+'),
    (1, N'Rafael Gomes',         '16899535009', '48991230004', CAST('1979-01-30' AS date), 'M', 'AB+'),
    (2, N'Juliana Pereira',      '71428793860', '11991230005', CAST('1990-09-09' AS date), 'F', 'A-'),
    (2, N'Lucas Ribeiro',        '09394687010', '11991230006', CAST('2000-05-17' AS date), 'M', 'O-'),
    (2, N'Patrícia Moura',       '33725418090', '11991230007', CAST('1987-12-05' AS date), 'F', 'B-'),
    (2, N'Thiago Barbosa',       '86288366757', '11991230008', CAST('1995-04-21' AS date), 'M', 'O+')
) AS t (HIdx, Nome, Documento, Telefone, Nascimento, Sexo, Tipo);

INSERT INTO Doadores (Id, Nome, Documento, Telefone, DataNascimento, UsuarioId, HemocentroId, Sexo, TipoSanguineo)
SELECT x.LeadId, l.Nome, l.Documento, l.Telefone, l.Nascimento, NULL, h.Id, l.Sexo, l.Tipo
FROM #leads l
JOIN #hemos h ON h.Idx = ((l.HIdx - 1) % @qtdHemos) + 1
CROSS APPLY (SELECT CAST(SUBSTRING(HASHBYTES('MD5', 'seed-lead-' + l.Documento + CAST(h.Id AS varchar(36))), 1, 16) AS uniqueidentifier) AS LeadId) x
WHERE NOT EXISTS (SELECT 1 FROM Doadores d WHERE d.Id = x.LeadId);

-- Doadores que cada hemocentro enxerga: todos os usuários + os leads do próprio hemocentro
SELECT h.Idx, d.Id AS DoadorId, d.TipoSanguineo,
       ROW_NUMBER() OVER (PARTITION BY h.Idx ORDER BY d.Nome, d.Id) AS rn,
       COUNT(*) OVER (PARTITION BY h.Idx) AS cnt
INTO #pool
FROM #hemos h
JOIN Doadores d ON d.UsuarioId IS NOT NULL OR d.HemocentroId = h.Id;

SELECT TOP (18) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i INTO #n FROM sys.all_objects;

-- Agendamentos concluídos: um a cada 4 dias, distribuídos entre os doadores do hemocentro
SELECT h.Idx, h.Id AS HemoId, h.NomeFantasia, n.i,
       2 + (n.i - 1) * 4 AS DiasAtras,
       h.HoraAbre + ((n.i - 1) % CASE WHEN h.HoraFecha > h.HoraAbre AND h.HoraFecha - h.HoraAbre < 5
                                     THEN h.HoraFecha - h.HoraAbre ELSE 5 END) AS Hora,
       p.DoadorId, p.TipoSanguineo,
       CAST(SUBSTRING(HASHBYTES('MD5', 'seed-ag-' + CAST(h.Id AS varchar(36)) + '-' + CAST(n.i AS varchar(5))), 1, 16) AS uniqueidentifier) AS AgId
INTO #slots
FROM #hemos h
CROSS JOIN #n n
JOIN #pool p ON p.Idx = h.Idx AND p.rn = ((n.i - 1) % p.cnt) + 1;

ALTER TABLE #slots ADD AgendadoPara datetime2 NULL;
UPDATE #slots SET AgendadoPara = DATEADD(HOUR, Hora, DATEADD(DAY, -DiasAtras, @hoje));

INSERT INTO Agendamentos (Id, AtualizadoEm, CriadoEm, AgendadoPara, Status, HemocentroId, DoadorId)
SELECT s.AgId, s.AgendadoPara, DATEADD(DAY, -7, s.AgendadoPara), s.AgendadoPara, 'CONCLUIDO', s.HemoId, s.DoadorId
FROM #slots s
WHERE NOT EXISTS (SELECT 1 FROM Agendamentos a WHERE a.Id = s.AgId);

-- Uma bolsa por agendamento (sangue total vale 35 dias; as mais recentes ainda estão em análise)
INSERT INTO BolsasSangue (Id, Codigo, TesteSorologico, TipoComponente, FatorRh, TipoSanguineo, Volume, Status,
                          CriadoEm, AtualizadoEm, AgendamentoId, DataColeta, DataValidade, Destino, LocalColeta, ResponsavelColeta)
SELECT c.BolsaId,
       'HEMO-' + CAST(s.Idx AS varchar(2)) + '-' + RIGHT('000' + CAST(s.i AS varchar(3)), 3),
       N'Negativo Geral',
       CHOOSE((s.i % 5) + 1, N'Sangue Total', N'Sangue Total', N'Conc. Hemácias', N'Plasma', N'Plaquetas'),
       CASE RIGHT(c.TipoCompleto, 1) WHEN '+' THEN N'Positivo' ELSE N'Negativo' END,
       LEFT(c.TipoCompleto, LEN(c.TipoCompleto) - 1),
       CAST(400 + ((s.i % 4) * 15) AS real),
       CASE WHEN s.DiasAtras <= 6 THEN 'EM_ANALISE'
            WHEN DATEADD(DAY, 35, CAST(CAST(s.AgendadoPara AS date) AS datetime2)) < @hoje THEN 'VENCIDA'
            WHEN s.i % 7 = 4 THEN 'DESCARTADA'
            ELSE 'DISPONIVEL' END,
       s.AgendadoPara,
       NULL,
       s.AgId,
       CAST(CAST(s.AgendadoPara AS date) AS datetime2),
       DATEADD(DAY, 35, CAST(CAST(s.AgendadoPara AS date) AS datetime2)),
       CHOOSE((s.i % 3) + 1, N'Hospital Central', N'Hospital Santa Catarina', N'Pronto Atendimento Municipal'),
       s.NomeFantasia,
       CHOOSE((s.i % 3) + 1, N'Dra. Helena Prado', N'Enf. João Mendes', N'Téc. Camila Duarte')
FROM #slots s
CROSS APPLY (SELECT CAST(SUBSTRING(HASHBYTES('MD5', 'seed-bolsa-' + CAST(s.AgId AS varchar(36))), 1, 16) AS uniqueidentifier) AS BolsaId,
                    COALESCE(NULLIF(s.TipoSanguineo, ''),
                             CHOOSE((ABS(CHECKSUM(s.AgId)) % 8) + 1, 'O+', 'A+', 'B+', 'O-', 'A-', 'AB+', 'B-', 'AB-')) AS TipoCompleto) c
WHERE NOT EXISTS (SELECT 1 FROM BolsasSangue b WHERE b.Id = c.BolsaId)
  AND NOT EXISTS (SELECT 1 FROM BolsasSangue b WHERE b.AgendamentoId = s.AgId);

-- Agendamentos cancelados e agendados (sem bolsa). Dias negativos = passado, positivos = futuro.
SELECT * INTO #extras FROM (VALUES
    (1, 'e1',  -1, 'AGENDADO'),   -- já passou do horário e aguarda conclusão
    (2, 'e2',   6, 'AGENDADO'),
    (3, 'e3',  10, 'AGENDADO'),
    (4, 'e4',  14, 'AGENDADO'),
    (5, 'e5',  18, 'AGENDADO'),
    (6, 'e6', -13, 'CANCELADO'),
    (7, 'e7', -30, 'CANCELADO'),
    (8, 'e8',   8, 'CANCELADO')
) AS t (Pos, Chave, Dias, Status);

INSERT INTO Agendamentos (Id, AtualizadoEm, CriadoEm, AgendadoPara, Status, HemocentroId, DoadorId)
SELECT x.AgId,
       CASE WHEN e.Status = 'CANCELADO' THEN CASE WHEN e.Dias < 0 THEN DATEADD(DAY, -2, x.Quando) ELSE @hoje END END,
       DATEADD(DAY, -7, CASE WHEN e.Dias < 0 THEN x.Quando ELSE @hoje END),
       x.Quando, e.Status, h.Id, p.DoadorId
FROM #hemos h
CROSS JOIN #extras e
JOIN #pool p ON p.Idx = h.Idx AND p.rn = ((e.Pos + 5) % p.cnt) + 1
CROSS APPLY (SELECT CAST(SUBSTRING(HASHBYTES('MD5', 'seed-ag-' + e.Chave + '-' + CAST(h.Id AS varchar(36))), 1, 16) AS uniqueidentifier) AS AgId,
                    DATEADD(HOUR, h.HoraAbre + (ABS(e.Dias) % CASE WHEN h.HoraFecha > h.HoraAbre AND h.HoraFecha - h.HoraAbre < 5
                                                                  THEN h.HoraFecha - h.HoraAbre ELSE 5 END),
                            DATEADD(DAY, e.Dias, @hoje)) AS Quando) x
WHERE NOT EXISTS (SELECT 1 FROM Agendamentos a WHERE a.Id = x.AgId);

-- Depoimentos de exemplo, escritos pelos usuários que já existem
SELECT ROW_NUMBER() OVER (ORDER BY Nome, Id) AS rn, UsuarioId INTO #autores
FROM Doadores WHERE UsuarioId IS NOT NULL;

INSERT INTO #autores (rn, UsuarioId)
SELECT 100 + Idx, UsuarioId FROM #hemos WHERE UsuarioId IS NOT NULL;

INSERT INTO Depoimentos (Id, Titulo, Conteudo, CriadoEm, AtualizadoEm, AutorId)
SELECT CAST(SUBSTRING(HASHBYTES('MD5', 'seed-dep-' + t.Chave), 1, 16) AS uniqueidentifier),
       t.Titulo, t.Conteudo, DATEADD(DAY, -t.Dias, @hoje), NULL, a.UsuarioId
FROM (VALUES
    ('a', 1,   N'Minha primeira doação', N'Tinha medo de agulha, mas a equipe foi muito atenciosa e tudo foi rápido. Sair de lá sabendo que posso ajudar até três pessoas não tem preço.', 40),
    ('b', 1,   N'Agendar ficou fácil',    N'Agendei pelo site em poucos minutos e fui atendido no horário marcado. Recomendo a todos que puderem doar.', 18),
    ('c', 101, N'Obrigado, doadores!',    N'Nosso estoque se mantém graças a quem dedica uma manhã para doar. Cada bolsa faz diferença para quem precisa.', 25),
    ('d', 102, N'Uma rotina que salva vidas', N'Receber doadores todos os dias nos lembra do quanto a solidariedade move a nossa comunidade.', 9)
) AS t (Chave, AutorRn, Titulo, Conteudo, Dias)
JOIN #autores a ON a.rn = t.AutorRn
WHERE NOT EXISTS (SELECT 1 FROM Depoimentos d WHERE d.Id = CAST(SUBSTRING(HASHBYTES('MD5', 'seed-dep-' + t.Chave), 1, 16) AS uniqueidentifier));

COMMIT;

SELECT (SELECT COUNT(*) FROM Doadores) AS Doadores,
       (SELECT COUNT(*) FROM Agendamentos) AS Agendamentos,
       (SELECT COUNT(*) FROM BolsasSangue) AS Bolsas,
       (SELECT COUNT(*) FROM Depoimentos) AS Depoimentos;
