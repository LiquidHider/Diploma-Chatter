PRINT 'Creating table: Identities...'
CREATE TABLE [Identities]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[Email] NVARCHAR(20) NOT NULL,
	[UserTag] NVARCHAR(20) NOT NULL,
	[PasswordHash] NVARCHAR(50) NOT NULL,
	[PasswordKey] NVARCHAR(50) NOT NULL,
	[UserID] UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT [PK_Identity_Id] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Identity_ChatUser] FOREIGN KEY ([UserID]) REFERENCES [dbo].[ChatUsers](ID) ON DELETE CASCADE
)
PRINT 'Creating nonclustered index: IX_Identities_UserTag...'
CREATE NONCLUSTERED INDEX [IX_Identities_UserTag] ON [dbo].[Identities]([UserTag])
PRINT 'Nonclustered index successfully created.'
PRINT 'Table successfully created.'