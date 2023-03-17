PRINT 'Creating table: GroupChats...'
CREATE TABLE [GroupChats]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[Name] NVARCHAR(20) NOT NULL,
	[Description] NVARCHAR(100) NULL,

	CONSTRAINT [PK_GroupChat_Id] PRIMARY KEY([ID])
)
PRINT 'Creating nonclustered index: IX_GroupChats_Name...'
CREATE NONCLUSTERED INDEX [IX_GroupChats_Name] ON [dbo].[GroupChats]([Name])
PRINT 'Nonclustered index successfully created.'
PRINT 'Table successfully created.'