USE [Longueur]
GO
/****** Object:  Table [dbo].[Comment]    Script Date: 06/02/2007 17:57:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comment](
	[CommentID] [int] IDENTITY(1,1) NOT NULL,
	[CommentText] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UserID] [int] NOT NULL,
	[DateTimeStamp] [datetime] NOT NULL,
	[QuoteID] [int] NOT NULL,
	[LastEditedDateTimeStamp] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 