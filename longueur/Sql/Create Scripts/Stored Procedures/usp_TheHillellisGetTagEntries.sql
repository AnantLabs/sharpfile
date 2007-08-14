set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TheHillellisGetTagEntries]
	@TagId INT
AS
BEGIN
	SET NOCOUNT ON

SELECT	h.Id, h.[Content], h.[DateTime], h.UserId, h.Title, s.[Name]
FROM	Tag t
JOIN	TheHillellis_Tag ta ON t.Id = ta.TagId
JOIN	TheHillellis h ON ta.EntryId = h.Id
JOIN	SiteUser s ON h.UserId = s.Id
WHERE	t.Id = @TagId
ORDER BY	h.[DateTime] DESC
   
SELECT Id, [Name], [Image]
FROM Tag
WHERE Id = @TagId
 
END