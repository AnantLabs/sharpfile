ALTER PROCEDURE dbo.usp_GetRecords
(
@MatchId INT = NULL,
@TournamentId INT = NULL
)
AS
SET NOCOUNT ON

DECLARE @Results TABLE 
(
	TournamentId INT,
	TournamentName VARCHAR(100),
	MatchId INT,
	GameId INT,
	[DateTime] DATETIME,
	Player1Id INT,
	Player1 VARCHAR(100),
	Player1Points INT,
	Player2Id INT,
	Player2 VARCHAR(100),
	Player2Points INT
)

INSERT INTO @Results
SELECT t.Id AS 'TournamentId', t.[Name] AS 'TournamentName', 
	m.Id AS 'MatchId', g.Id AS 'GameId', m.[DateTime],
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id ASC) AS 'Player1Id', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id ASC) AS 'Player1',
	(SELECT TOP 1 s.[Points] 
	FROM Score s 
	WHERE g.Id = s.GameId
	ORDER BY s.PlayerId ASC) AS 'Player1Points',
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id DESC) AS 'Player2Id', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id DESC) AS 'Player2',
	(SELECT TOP 1 s.[Points] 
	FROM Score s 
	WHERE g.Id = s.GameId
	ORDER BY s.PlayerId DESC) AS 'Player2Points'
FROM Game g
	LEFT JOIN Match m ON g.MatchId = m.Id
	LEFT JOIN Tournament t ON m.TournamentId = t.Id
ORDER BY t.Id ASC, m.Id DESC, g.Id DESC;

IF @TournamentId IS NOT NULL
BEGIN
	SELECT * 
	FROM @Results
	WHERE TournamentId = @TournamentId
END
ELSE IF @MatchId IS NOT NULL
BEGIN
	SELECT * 
	FROM @Results
	WHERE MatchId = @MatchId
END
ELSE
BEGIN	
	SELECT *
	FROM @Results
END

RETURN