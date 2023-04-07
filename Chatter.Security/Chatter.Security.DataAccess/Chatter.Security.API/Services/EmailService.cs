using Chatter.Security.API.Interfaces;
using Chatter.Security.Common;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Chatter.Security.API.Models;
using Chatter.Security.API.RabbitMQ;

namespace Chatter.Security.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ServiceResult SendCongratulationsMessageToNewUser(string email)
        {
            var result = new ServiceResult();
            var messageModel = new SendEmailModel()
            {
                To = email,
                Subject = "Welcome to Chatter messenger!",
                Body = "Welcome to chatter messenger!",
                IsBodyHtml = false
            };

            SendMessageToEmailService(messageModel);

            return result;
        }

        private bool SendMessageToEmailService(SendEmailModel emailModel) 
        {
            var rabbitMqConfig = _configuration.GetSection(RabbitMQOptions.RabbittMQConfigSectionName);
            ConnectionFactory factory = new()
            {
                HostName = rabbitMqConfig.GetSection("HostName").Value,
                UserName = rabbitMqConfig.GetSection("UserName").Value,
                Password = rabbitMqConfig.GetSection("Password").Value,
                VirtualHost = rabbitMqConfig.GetSection("VirtualHost").Value,
                ClientProvidedName = rabbitMqConfig.GetSection("ClientProvidedName").Value,
                Port = int.Parse(rabbitMqConfig.GetSection("Port").Value)
            };
            var rabbitMqRoutingConfig = rabbitMqConfig.GetSection(RabbitMQOptions.RabbittMQRoutingConfigSectionName);
            IConnection connection = factory.CreateConnection();

            using IModel channel = connection.CreateModel();

            string exchange = rabbitMqRoutingConfig.GetSection("Exchange").Value;
            string routingKey = rabbitMqRoutingConfig.GetSection("RoutingKey").Value;
            string queueName = rabbitMqRoutingConfig.GetSection("Queue").Value;


            channel.ExchangeDeclare(exchange, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false);
            channel.QueueBind(queueName, exchange, routingKey, null);


            var modelBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(emailModel));

            channel.BasicPublish(exchange, routingKey, body: modelBytes);

            channel.Close();
            connection.Close();
            return true;
        }
    }
}
