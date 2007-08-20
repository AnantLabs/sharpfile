USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisUpdate]    Script Date: 08/20/2007 23:15:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_TheHillellisUpdate]
	(
	@Id INT,
	@Title VARCHAR(255),
	@Content TEXT,
	@UserId INT,
	@TagIds VARCHAR(512)
	)
AS
	SET NOCOUNT ON
	
DECLARE @TagIdTable TABLE(Id INT);

BEGIN TRANSACTION UpdateEntry
	UPDATE TheHillellis 
	SET Title = @Title, 
		Content = @Content, 
		UserId = @UserId 
	WHERE Id = @Id;

	INSERT INTO @TagIdTable(Id)
	SELECT Data FROM dbo.fn_Split(@TagIds, ',');

	DELETE FROM TheHillellis_Tag WHERE EntryId = @Id;	
	INSERT INTO TheHillellis_Tag (EntryId, TagId)
	SELECT @Id AS EntryId, Id AS TagId
	FROM @TagIdTable
COMMIT TRANSACTION UpdateEntry
	
RETURN