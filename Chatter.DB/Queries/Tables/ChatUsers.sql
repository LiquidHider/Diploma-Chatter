PRINT 'Creating table: ChatUsers...'
CREATE TABLE [ChatUsers]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[LastName] NVARCHAR(20) NOT NULL,
	[FirstName] NVARCHAR(20) NOT NULL,
	[Patronymic] NVARCHAR(20) NULL,
	[UniversityName] NVARCHAR(50) NULL,
	[UniversityFaculty] NVARCHAR(50) NULL,
	[JoinedUtc] DATETIME NOT NULL,
	[LastActive] DATETIME NOT NULL,
	[IsBlocked] BIT NOT NULL,
	[BlockedUntil] DATETIME NULL,

	CONSTRAINT [PK_ChatUser_Id] PRIMARY KEY ([ID])
)
PRINT 'Table successfully created.'