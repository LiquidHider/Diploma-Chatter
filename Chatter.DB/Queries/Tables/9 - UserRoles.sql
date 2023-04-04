PRINT 'Creating table: UserRoles...'
CREATE TABLE [UserRoles]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[UserID] UNIQUEIDENTIFIER NOT NULL,
	[UserRole] INT NOT NULL,
	[UserRoleName] NVARCHAR(20) NOT NULL

	CONSTRAINT [PK_UserRole_Id] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_UserRole_ChatUser_ID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Identities](ID) ON DELETE CASCADE,

)
PRINT 'Table successfully created.'