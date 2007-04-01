SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tournament]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Tournament](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Game]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Game](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MatchId] [int] NULL,
 CONSTRAINT [PK_Game_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Match]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Match](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TournamentId] [int] NULL,
	[DateTime] [datetime] NOT NULL CONSTRAINT [DF_Match_DateTime]  DEFAULT (getdate()),
 CONSTRAINT [PK_Match] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Score]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Score](
	[GameId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Points] [int] NOT NULL,
 CONSTRAINT [PK_Score] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC,
	[PlayerId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Player]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Player](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Player] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertMatch]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_InsertMatch] 
(
	@TournamentId INT = 1,
	@DateTime DATETIME = NULL,
	@Player1 INT = NULL,
	@Player1Points INT = 0,
	@Player2 INT = NULL,
	@Player2Points INT = 0
)
AS
SET NOCOUNT ON

DECLARE @MatchId INT;
DECLARE @GameId INT;

IF @DateTime IS NULL
BEGIN
	SET @DateTime = GETDATE();
END
	
IF @Player1 IS NOT NULL AND @Player2 IS NOT NULL
BEGIN
	BEGIN TRANSACTION t
		INSERT INTO Match (TournamentId, [DateTime])
		VALUES (@TournamentId, @DateTime);
		
		SET @MatchId = SCOPE_IDENTITY();
		
		IF @MatchId IS NULL
		BEGIN
			ROLLBACK TRANSACTION t
		END
		ELSE
		BEGIN
			INSERT INTO Game (MatchId)
			VALUES (@MatchId);
		
			SET @GameId = SCOPE_IDENTITY();
			
			IF @GameId IS NULL
			BEGIN
				ROLLBACK TRANSACTION t
			END
			ELSE
			BEGIN
				INSERT INTO Score (GameId, PlayerId, Points)
				VALUES (@GameId, @Player1, @Player1Points);
				
				INSERT INTO Score (GameId, PlayerId, Points)
				VALUES (@GameId, @Player2, @Player2Points);
				
				COMMIT TRANSACTION t;
			END
		END
END
	
RETURN
' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertGame]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_InsertGame]
(
	@MatchId INT = NULL,
	@Player1 INT = NULL,
	@Player1Points INT = 0,
	@Player2 INT = NULL,
	@Player2Points INT = 0
)
AS
SET NOCOUNT ON

DECLARE @GameId INT;
	
IF @Player1 IS NOT NULL AND @Player2 IS NOT NULL AND @MatchId IS NOT NULL
BEGIN
	BEGIN TRANSACTION t
		INSERT INTO Game (MatchId)
		VALUES (@MatchId);
	
		SET @GameId = @@IDENTITY;
		
		IF @GameId IS NULL
		BEGIN
			ROLLBACK TRANSACTION t
		END
		ELSE
		BEGIN
			INSERT INTO Score (GameId, PlayerId, Points)
			VALUES (@GameId, @Player1, @Player1Points);
			
			INSERT INTO Score (GameId, PlayerId, Points)
			VALUES (@GameId, @Player2, @Player2Points);
			
			COMMIT TRANSACTION t;
		END	
END
	
RETURN' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetGames]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_GetGames]
(
	@MatchId INT = NULL
)
AS
SET NOCOUNT ON
	
SELECT	Id,
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id ASC) AS ''Player1Id'', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id ASC) AS ''Player1'',
	(SELECT TOP 1 s.[Points] 
	FROM Score s 
	WHERE g.Id = s.GameId
	ORDER BY s.PlayerId ASC) AS ''Player1Points'',
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id DESC) AS ''Player2Id'', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
	WHERE g.Id = s.GameId
	ORDER BY p.Id DESC) AS ''Player2'',
	(SELECT TOP 1 s.[Points] 
	FROM Score s 
	WHERE g.Id = s.GameId
	ORDER BY s.PlayerId DESC) AS ''Player2Points''
FROM	Game g 
WHERE	MatchId = @MatchId
ORDER BY g.Id ASC;
	
RETURN' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetMatches]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_GetMatches]
(
	@TournamentId INT = 1
)
AS
SET NOCOUNT ON
	
SELECT m.Id, [DateTime],
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id ASC) AS ''Player1Id'', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id ASC) AS ''Player1'',
	(SELECT TOP 1 p.Id 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id DESC) AS ''Player2Id'', 
	(SELECT TOP 1 p.[Name] 
	FROM Score s 
		JOIN Player p ON s.PlayerId = p.Id
		JOIN Game g ON s.GameId = g.Id
	WHERE g.Id = s.GameId AND m.Id = g.MatchId
	ORDER BY p.Id DESC) AS ''Player2''
FROM	Match m
WHERE	TournamentId = @TournamentId
GROUP BY m.Id, m.DateTime
ORDER BY [DateTime] DESC;
	
RETURN' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetTournaments]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_GetTournaments]
AS
SET NOCOUNT ON
	
SELECT		Id, [Name]
FROM		Tournament
ORDER BY	Id ASC;
	
RETURN' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertTournament]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_InsertTournament] 
(
@Name VARCHAR(100)
)
AS
SET NOCOUNT ON

INSERT INTO Tournament ([Name])
VALUES (@Name);

RETURN' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetPlayers]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[usp_GetPlayers]
AS
SET NOCOUNT ON
	
SELECT	Id, [Name]
FROM	Player
ORDER BY Id ASC;
	
RETURN
' 
END
