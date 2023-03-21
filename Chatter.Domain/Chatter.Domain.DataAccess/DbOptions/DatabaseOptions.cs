namespace Chatter.Domain.DataAccess.DbOptions
{
    internal class DatabaseOptions
    {
        public const string ConfigurationSectionName = "ConnectionStrings";

        public string? ChatterDbConnection { get; set; }
    }
}
