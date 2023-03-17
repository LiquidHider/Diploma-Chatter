PRINT 'Start: Creating login (sysadmin)...'
BEGIN TRY
	USE [chatter]
	CREATE LOGIN [sadmin] WITH PASSWORD=N'jcAx_bd^2sa12eG~@1jc', DEFAULT_DATABASE=[chatter], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
	ALTER SERVER ROLE [sysadmin] ADD MEMBER [sadmin]
	CREATE USER [administrator] FOR LOGIN [sadmin]
	PRINT 'Result: Login sadmin (sysadmin) successfully created.'
END TRY
BEGIN CATCH
	PRINT 'Error: Cannot create login: ' + ERROR_MESSAGE()
END CATCH