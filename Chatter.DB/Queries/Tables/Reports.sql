PRINT 'Creating table: Reports...'
CREATE TABLE [Reports]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[ReportedUserID] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(20) NOT NULL,
	[Message] NVARCHAR(500) NOT NULL,

	CONSTRAINT [PK_Report_Id] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Report_ChatUser] FOREIGN KEY ([ReportedUserID]) REFERENCES [dbo].[ChatUsers](ID)
)
PRINT 'Table successfully created.'