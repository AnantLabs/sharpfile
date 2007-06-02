USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisGet]    Script Date: 06/02/2007 18:25:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[usp_TheHillellisGet]
(
	@UserId INT = NULL,
	@Rowcount INT = NULL
)
AS
	SET NOCOUNT ON

IF @Rowcount IS NOT NULL
BEGIN
	SET ROWCOUNT @Rowcount
END

IF @UserId IS NULL
BEGIN
	SELECT	h.Id, h.[Content], h.[DateTime], u.Id AS 'UserId', u.[Name], h.Title
	FROM	TheHillellis h
		JOIN SiteUser u ON h.UserId = u.Id
	ORDER BY h.[DateTime] DESC
END
ELSE
BEGIN
	SELECT	h.Id, h.[Content], h.[DateTime], u.Id AS 'UserId', u.[Name], h.Title
	FROM	TheHillellis h
		JOIN SiteUser u ON h.UserId = u.Id
	WHERE h.UserId = @UserId
		AND h.Id NOT IN (
			SELECT EntryId
			FROM TheHillellis_Archive
		)
	ORDER BY h.[DateTime] DESC
END
	
	RETURN

