PRINT 'Start: Creating tables...'
USE [chatter]
BEGIN TRY

--chat users
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\ChatUsers.sql

--identities
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\Identities.sql

--reports
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\Reports.sql

--photos
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\Photos.sql

--group chats
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\GroupChats.sql

--messages
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\Messages.sql

--user joined groups
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\UserJoinedGroups.sql

--blocked group chat users
:r D:\Repos\Chatter\Chatter.DB\Queries\Tables\BlockedGroupChatUsers.sql

END TRY
BEGIN CATCH
PRINT 'Error: Cannot create tables: ' + ERROR_MESSAGE()
END CATCH