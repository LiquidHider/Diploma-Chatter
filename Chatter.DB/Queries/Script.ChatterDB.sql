PRINT 'Chatter data base build...'
BEGIN TRAN BUILD_CHATTER_DB
BEGIN TRY
:r  D:\Repos\Chatter\Chatter.DB\Queries\CreateDB.sql

:r  D:\Repos\Chatter\Chatter.DB\Queries\CreateDBAdmin.sql

:r  D:\Repos\Chatter\Chatter.DB\Queries\CreateTables.sql
COMMIT TRAN BUILD_CHATTER_DB
END TRY
BEGIN CATCH
	ROLLBACK TRAN BUILD_CHATTER_DB
END CATCH


PRINT 'Chatter database build completed.'