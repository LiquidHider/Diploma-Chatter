PRINT 'Creating table: BlockedGroupChatUsers...'
CREATE TABLE [BlockedGroupChatUsers]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[GroupID] UNIQUEIDENTIFIER NOT NULL,
	[UserID] UNIQUEIDENTIFIER NOT NULL,
	[BlockedUntil] DATETIME NULL,

	CONSTRAINT [PK_BlockedGroupChatUser] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_BlockedGroupChatUser_GroupChat] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[GroupChats](ID) ON DELETE CASCADE,
	CONSTRAINT [FK_BlockedGroupChatUser_ChatUser] FOREIGN KEY ([UserID]) REFERENCES [dbo].[ChatUsers](ID) ON DELETE CASCADE
)
PRINT 'Table successfully created.'