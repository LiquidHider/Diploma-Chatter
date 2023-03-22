using Chatter.Domain.DataAccess.DbOptions;
using Chatter.Domain.Tests.IntegrationTests.Database;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Xml.Linq;

namespace Chatter.Domain.IntegrationTests.Database.DatabaseFixture
{
    internal class DatabaseFixture
    {
        public readonly DatabaseOptions dbOptions;

        private readonly ConnectionOptions connectionOptions;

        public DatabaseFixture()
        {
            connectionOptions = GetOptions();
            dbOptions = new DatabaseOptions()
            {
                ChatterDbConnection = CreateConnectionString(false)
            };
        }
        public void EnsureCreated(bool createEmptyDb = true)
        {
            string queriesPath = @"..\..\..\..\..\Chatter.DB\Queries\Tables";
            string createDbQuery = $"CREATE DATABASE [{connectionOptions.InitialCatalog}]";
            string populateDbQuery = @"..\..\..\..\..\Chatter.DB\Queries\PopulateDatabase.sql";
            var sqlCreateTablesQueriesPaths = Directory.EnumerateFiles(queriesPath);

            //connection to create database
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

                if (createEmptyDb is true)
                {
                    db.Execute(new CommandDefinition(File.ReadAllText(populateDbQuery)));
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
    

        public void EnsureDeleted() 
        {
            
        }
    }
}
