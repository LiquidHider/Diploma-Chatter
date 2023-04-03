PRINT 'Start: Creating tables...'

BEGIN TRY

--chat users
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\1 - ChatUsers.sql

--identities
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\2 - Identities.sql

--reports
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\3 - Reports.sql

--photos
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\4 - Photos.sql

--group chats
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\5 - GroupChats.sql

--messages
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\6 - Messages.sql

--user joined groups
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\7 - UserJoinedGroups.sql

--blocked group chat users
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\8 - BlockedGroupChatUsers.sql

--user roles
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\9 - UserRoles.sql

END TRY
BEGIN CATCH
PRINT 'Error: Cannot create tables: ' + ERROR_MESSAGE()
END CATCH