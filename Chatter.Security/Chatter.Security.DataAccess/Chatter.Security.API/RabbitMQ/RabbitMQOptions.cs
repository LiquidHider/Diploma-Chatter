namespace Chatter.Security.API.RabbitMQ
{
    public class RabbitMQOptions
    {
        public const string RabbittMQConfigSectionName = "RabbitMQ";

        public const string RabbittMQRoutingConfigSectionName = "Routing";

        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public string ClientProvidedName { get; set; }

        public int Port { get; set; }
    }
}
