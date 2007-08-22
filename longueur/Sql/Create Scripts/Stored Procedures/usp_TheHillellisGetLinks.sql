 USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisGetLinks]    Script Date: 06/02/2007 18:25:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_TheHillellisGetLinks]
AS
BEGIN
	SET NOCOUNT ON

	SELECT	l.Id, l.Href, l.Title, l.Description
	FROM	Link l
	ORDER BY l.Title ASC
    
END
