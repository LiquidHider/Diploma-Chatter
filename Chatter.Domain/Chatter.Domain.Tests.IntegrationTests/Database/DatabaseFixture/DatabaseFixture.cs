using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.Tests.IntegrationTests.Database;
using Chatter.Domain.Tests.IntegrationTests.Helper;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace Chatter.Domain.IntegrationTests.Database.DatabaseFixture
{
    internal class DatabaseFixture
    {
        public readonly DatabaseOptions dbOptions;

        private readonly ConnectionOptions connectionOptions;
        private const string VerifyDbExistsSql = @"
                SELECT
                CASE WHEN EXISTS 
                (
                    SELECT * 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = @Schema
                    AND  TABLE_NAME = @TableName
                )
                    THEN 1
                    ELSE 0
                END";

        public DatabaseFixture()
        {
            connectionOptions = GetOptions();
            dbOptions = new DatabaseOptions()
            {
                ChatterDbConnection = CreateConnectionString(false)
            };
        }

        public bool IsDatabaseCreated() 
        {
            using (IDbConnection db = new SqlConnection(CreateConnectionString(false)))
            {
               var commands = DataHelper.TableNameMap
               .Select(x =>
               {
                   var tableSchemaPair = x.Split(".");
                   return new { TableName = x, Command = new CommandDefinition(VerifyDbExistsSql, new { Schema = tableSchemaPair[0], TableName = tableSchemaPair[1] }) };
               });
               foreach (var item in commands)
               {
                   var tableExists = db.ExecuteScalar<int>(item.Command);
               
                   if (tableExists != 1)
                   {
                       throw new Exception($"Table was not created: {item.TableName}");
                   }
               }
            }

            return true;
        }

        public void EnsureCreated(bool createEmptyDb = true)
        {
            if (IsDatabaseCreated() is true) 
            {
                return;
            }

            string queriesPath = @"..\..\..\..\..\Chatter.DB\Queries\Tables";
            string createDbQuery = $"CREATE DATABASE [{connectionOptions.InitialCatalog}]";
            
            var sqlCreateTablesQueriesPaths = Directory.EnumerateFiles(queriesPath);
            


            //connection to create database using master db
            using (IDbConnection db = new SqlConnection(CreateConnectionString(true)))
            {
                db.Execute(new CommandDefinition(createDbQuery));
                db.Close();
            }

            //connection to create tables
            using (IDbConnection db = new SqlConnection(dbOptions.ChatterDbConnection))
            {
                
                foreach (var cmd in sqlCreateTablesQueriesPaths)
                {
                    var query = File.ReadAllText(cmd);
                    db.Execute(new CommandDefinition(query));
                }

                if (createEmptyDb is false)
                {
                    PopulateDatabase();
                }
                db.Close();
            }
        }

        public ConnectionOptions GetOptions() 
        {
            const string OptionsFilename = @"..\..\..\testsettings.json";
            var json = File.ReadAllText(OptionsFilename);
            return JsonSerializer.Deserialize<ConnectionOptions>(json);
        }

        private string CreateConnectionString(bool useMaster)
        {
            var connectionArgs = new List<string>();

            connectionArgs.Add($"Data Source={connectionOptions.DataSource}");

            connectionArgs.Add($"Initial Catalog={(useMaster ? "master" : connectionOptions.InitialCatalog)}");
            

            if (connectionOptions.IntegratedSecurity == true)
            {
                connectionArgs.Add($"Integrated Security={connectionOptions.IntegratedSecurity}");
            }
            else
            {
                connectionArgs.Add($"User={connectionOptions.UserId}");
                connectionArgs.Add($"Password={connectionOptions.Password}");
            }

            return string.Join(";", connectionArgs);
        }
    
        public void PopulateDatabase() 
        {
            string populateDbQuery = @"..\..\..\..\..\Chatter.DB\Queries\PopulateDatabase.sql";

            using (IDbConnection db = new SqlConnection(dbOptions.ChatterDbConnection))
            {
                db.Execute(new CommandDefinition(File.ReadAllText(populateDbQuery)));
            }
        }
    }
}
