USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisInsert]    Script Date: 08/20/2007 23:15:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_TheHillellisInsert] 
	(
	@Content TEXT,
	@UserId INT,
	@Title VARCHAR(256),
	@DateTime DateTime,
	@TagIds VARCHAR(512)
	)
AS
	SET NOCOUNT ON

DECLARE @NonArchivedCount INT;
DECLARE @Today DATETIME;
DECLARE @EarliestNonArchivedEntryDate DATETIME;
DECLARE @NonArchivedEntryTable TABLE(Id INT, [DateTime] DATETIME);
DECLARE @TagIdTable TABLE(Id INT);
DECLARE @EntryId INT;

SET @Today = GETDATE();

-- Give us a nice table varibale to grab data from.
INSERT INTO @NonArchivedEntryTable
	SELECT	h.Id, h.[DateTime]
	FROM	dbo.TheHillellis h
	WHERE	h.Id NOT IN (
		SELECT EntryId
		FROM TheHillellis_Archive
		)
	AND h.UserId = @UserId

-- Look up the earliest non-archived entry.
SET @EarliestNonArchivedEntryDate = (
	SELECT	TOP 1 [DateTime]
	FROM	@NonArchivedEntryTable
	ORDER BY [DateTime] ASC
	);

SET @NonArchivedCount = (
	SELECT	COUNT(Id)
	FROM	@NonArchivedEntryTable
	);

-- Make sure that the last non-archived entry is a week ago
IF DATEDIFF(d, @EarliestNonArchivedEntryDate, @Today) >= 7 AND @NonArchivedCount > 2
BEGIN
	-- Get the latest non-archived entry date to insert into the archive table.
	DECLARE @LatestNonArchivedEntryDate DATETIME;
	SET @LatestNonArchivedEntryDate = (
		SELECT	TOP 1 [DateTime]
		FROM	@NonArchivedEntryTable
		ORDER BY [DateTime] DESC
		);	

	BEGIN TRANSACTION InsertArchive
		-- Add the archive.
		INSERT INTO Archive (StartDate, EndDate, UserId)
		VALUES (@EarliestNonArchivedEntryDate, @LatestNonArchivedEntryDate, @UserId)

		-- Grab the archive id.
		DECLARE @ArchiveId INT;
		SET @ArchiveId = IDENT_CURRENT('Archive');

		-- Insert the entry id's into the linking table.
		INSERT INTO TheHillellis_Archive (ArchiveId, EntryId)
		SELECT	@ArchiveId, Id
		FROM	@NonArchivedEntryTable

		-- Add our new entry into the TheHillellis table.
		INSERT INTO TheHillellis ([Content], UserId, Title, [DateTime])
		VALUES	(@Content, @UserId, @Title, @DateTime)

		SET @EntryId = SCOPE_IDENTITY();

		INSERT INTO @TagIdTable(Id)
		SELECT Data FROM dbo.fn_Split(@TagIds, ',');

		INSERT INTO TheHillellis_Tag (EntryId, TagId)
		SELECT @EntryId AS EntryId, Id AS TagId
		FROM @TagIdTable
	COMMIT TRANSACTION InsertArchive
END
ELSE
BEGIN
	BEGIN TRANSACTION InsertEntry
		SET @EntryId = SCOPE_IDENTITY();

		-- Add our new entry into the TheHillellis table.
		INSERT INTO TheHillellis ([Content], UserId, Title, [DateTime])
		VALUES	(@Content, @UserId, @Title, @DateTime);

		INSERT INTO @TagIdTable(Id)
		SELECT Data FROM dbo.fn_Split(@TagIds, ',');

		INSERT INTO TheHillellis_Tag (EntryId, TagId)
		SELECT @EntryId AS EntryId, Id AS TagId
		FROM @TagIdTable
	COMMIT TRANSACTION InsertEntry
END

RETURN