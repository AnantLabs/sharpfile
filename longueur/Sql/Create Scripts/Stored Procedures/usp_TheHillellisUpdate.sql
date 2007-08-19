USE [Longueur]
GO
/****** Object:  StoredProcedure [dbo].[usp_TheHillellisUpdate]    Script Date: 08/19/2007 22:35:51 ******/
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
	
UPDATE TheHillellis 
SET Title = @Title, 
	Content = @Content, 
	UserId = @UserId 
WHERE Id = @Id;

INSERT INTO @TagIdTable(Id)
SELECT Id FROM dbo.fn_Split(@TagIds, ',');

DELETE FROM TheHillellis_Tag WHERE EntryId = @Id;	
INSERT INTO TheHillellis_Tag (EntryId, TagId)
SELECT @Id AS EntryId, Id AS TagId
FROM @TagIdTable
	
	RETURN
