/****** Object:  Table [dbo].[CrawlResults]    Script Date: 2/03/2015 5:17:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CrawlRun](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Domain] [nvarchar](250) NOT NULL,
	[Metadata] [nvarchar](2000) NULL,
	[Created] [datetime] NOT NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[CrawlResults](
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Domain] [nvarchar](250) NOT NULL,
	[Url] [nvarchar](2000) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Code] [int] NOT NULL,
	[HasJavascriptErrors] [bit] NOT NULL,
	[JavascriptErrors] [nvarchar](2000) NULL,
	[TimeTakenMs] [int] NOT NULL,
	[CrawlRunId] [int] NOT NULL FOREIGN KEY REFERENCES CrawlRun(Id)
) ON [PRIMARY]

GO