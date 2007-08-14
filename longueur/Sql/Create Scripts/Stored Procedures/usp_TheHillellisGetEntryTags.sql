set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TheHillellisGetEntryTags]
(
	@EntryId INT = 0
)
AS
BEGIN
	SET NOCOUNT ON

	SELECT	t.Id, t.[Name], t.[Image]
	FROM	TheHillellis_Tag tht
	JOIN	Tag t ON tht.TagId = t.Id
	WHERE	tht.EntryId = @EntryId
	ORDER BY t.[Name] DESC
    
END
