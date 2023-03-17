PRINT 'Creating table: UserJoinedGroups...'
CREATE TABLE [UserJoinedGroups]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[GroupID] UNIQUEIDENTIFIER NOT NULL,
	[UserID] UNIQUEIDENTIFIER NOT NULL,
	[UserRole] INT NOT NULL,

	CONSTRAINT [PK_UserJoinedGroup] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_UserJoinedGroup_GroupChat] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[GroupChats]([ID]) ON DELETE CASCADE,
	CONSTRAINT [FK_UserJoinedGroup_ChatUser] FOREIGN KEY ([UserID]) REFERENCES [dbo].[ChatUsers]([ID]) ON DELETE CASCADE
)
PRINT 'Table successfully created.'