PRINT 'Creating table: Photos...'
CREATE TABLE [Photos]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[Url] NVARCHAR(50) NOT NULL,
	[IsMain] BIT NOT NULL,
	[UserID] UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT [PK_Photo_Id] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Photo_ChatUser] FOREIGN KEY ([UserID]) REFERENCES [dbo].[ChatUsers]([ID]) ON DELETE CASCADE
)
PRINT 'Creating nonclustered index: IX_Photos_IsMain...'
CREATE NONCLUSTERED INDEX [IX_Photos_IsMain] ON [dbo].[Photos]([IsMain])
PRINT 'Nonclustered index successfully created.'
PRINT 'Table successfully created.'