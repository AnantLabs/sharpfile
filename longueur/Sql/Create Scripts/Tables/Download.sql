USE [Longueur]
GO
/****** Object:  Table [dbo].[Download]    Script Date: 06/02/2007 18:14:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Download](
	[Id] [int] NOT NULL,
	[IP] [varchar](15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Referrer] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[UserAgent] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Browser] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Platform] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[BrowserVersion] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[HostName] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DateTime] [datetime] NULL CONSTRAINT [DF_Download_DateTime]  DEFAULT (getdate())
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF 