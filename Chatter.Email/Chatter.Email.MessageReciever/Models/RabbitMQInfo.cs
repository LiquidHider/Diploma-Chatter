namespace Chatter.Email.MessageReciever.Models
{
    internal class RabbitMQInfo
    {
        public string HostName { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ClientProvidedName { get; set; }

        public string ExchangeName { get; set; }

        public string RoutingKey { get; set; }

        public string QueueName { get; set; }
    }
}
