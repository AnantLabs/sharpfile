set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TheHillellisGetTags]
AS
BEGIN
	SET NOCOUNT ON

	SELECT	t.Id, t.[Name], 
		(SELECT COUNT(tht.Id) FROM TheHillelis_Tag tht WHERE t.Id = tht.Id) AS 'Count'
	FROM	Tag t
	ORDER BY t.[Name] DESC
    
END 