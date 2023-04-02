namespace Chatter.Security.DataAccess.DbOptions
{
    public class DatabaseOptions
    {
        public const string ConfigurationSectionName = "ConnectionStrings";
        public const string ConnectionStringSectionName = "ChatterDbConnection";

        public string? ChatterDbConnection { get; set; }
    }
}
