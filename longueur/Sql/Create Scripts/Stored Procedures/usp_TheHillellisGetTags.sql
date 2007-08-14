set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TheHillellisGetTags]
AS
BEGIN
	SET NOCOUNT ON

	SELECT	t.Id, t.[Name], t.[Image], COUNT(ht.TagId) AS TagCount
	FROM	Tag t
	JOIN	TheHillellis_Tag ht ON t.Id = ht.TagId
	WHERE t.Id IN (SELECT TagId FROM TheHillellis_Tag)
	GROUP BY t.Id, t.Name, t.Image
	ORDER BY COUNT(ht.TagId) DESC, t.[Name] ASC
    
END

