PRINT 'Creating table: Messages...'
CREATE TABLE [Messages]
(
	[ID] UNIQUEIDENTIFIER  NOT NULL,
	[Body] NVARCHAR(4000) NOT NULL,
	[IsEdited] BIT NOT NULL,
	[Sent] DATETIME NOT NULL,
	[IsRead] BIT NOT NULL,
	[Sender] UNIQUEIDENTIFIER NOT NULL,
	[RecipientUser] UNIQUEIDENTIFIER NULL,
	[RecipientGroup] UNIQUEIDENTIFIER NULL,

	CONSTRAINT [PK_Message_Id] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Message_ChatUser_Sender] FOREIGN KEY ([Sender]) REFERENCES [dbo].[ChatUsers](ID),
	CONSTRAINT [FK_Message_ChatUser_Recipient] FOREIGN KEY ([RecipientUser]) REFERENCES [dbo].[ChatUsers](ID),
	CONSTRAINT [FK_Message_GroupChat] FOREIGN KEY ([RecipientGroup]) REFERENCES [dbo].[GroupChats](ID) ON DELETE CASCADE
)
PRINT 'Table successfully created.'