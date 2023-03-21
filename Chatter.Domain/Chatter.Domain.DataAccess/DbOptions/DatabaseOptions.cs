namespace Chatter.Domain.DataAccess.DbOptions
{
    public class DatabaseOptions
    {
        public const string ConfigurationSectionName = "ConnectionStrings";

        public string? ChatterDbConnection { get; set; }
    }
}
