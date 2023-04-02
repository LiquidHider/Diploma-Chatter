
namespace Chatter.Security.Tests.IntegrationTests.Database
{
    internal class ConnectionOptions
    {
        public string DataSource { get; set; }

        public string InitialCatalog { get; set; }

        public bool IntegratedSecurity { get; set; }

        public string? UserId { get; set; }

        public string? Password { get; set; }
    }
}
