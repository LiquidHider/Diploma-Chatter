PRINT 'Start: Creating database...'
BEGIN TRY
CREATE DATABASE [chatter]
PRINT 'Result: Database successfully created.'
END TRY
BEGIN CATCH
PRINT 'Error: Cannot create database: ' + ERROR_MESSAGE()
END CATCH
