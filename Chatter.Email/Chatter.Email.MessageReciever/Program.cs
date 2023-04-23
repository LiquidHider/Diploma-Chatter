using Chatter.Email.Core.Interfaces;
using Chatter.Email.Core.Models;
using Chatter.Email.Core.Service;
using Chatter.Email.MessageReciever.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
class Program {
    static void Main() {
     

        var configBuilder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json");

        var configuration = configBuilder.Build();

        var ClientConfig = configuration.GetSection("RabbitMQ");
        var SenderConfig = configuration.GetSection("Sender");

        var rabbitMqInfoModel = new RabbitMQInfo() 
        { 
            HostName = ClientConfig.GetSection("HostName").Value,
            Port = int.Parse(ClientConfig.GetSection("Port").Value),
            UserName = ClientConfig.GetSection("UserName").Value,
            Password = ClientConfig.GetSection("Password").Value,
            ClientProvidedName = ClientConfig.GetSection("ClientProvidedName").Value,
            ExchangeName = ClientConfig.GetSection("ExchangeName").Value,
            RoutingKey = ClientConfig.GetSection("RoutingKey").Value,
            QueueName = ClientConfig.GetSection("QueueName").Value
        };

        var senderModel = new Sender()
        {
            SmtpAddress = SenderConfig.GetSection("SmtpAddress").Value,
            PortNumber = int.Parse(SenderConfig.GetSection("PortNumber").Value),
            EnableSSL = bool.Parse(SenderConfig.GetSection("EnableSSL").Value),
            FromAddress = SenderConfig.GetSection("FromAddress").Value,
            FromPassword = SenderConfig.GetSection("FromPassword").Value

        };


        IEmailService emailService = new SmtpEmailService(senderModel);

        ConnectionFactory factory = new() { 
            HostName = rabbitMqInfoModel.HostName,
            Port = rabbitMqInfoModel.Port,
            UserName = rabbitMqInfoModel.UserName,
            Password = rabbitMqInfoModel.Password
        };

        factory.ClientProvidedName = rabbitMqInfoModel.ClientProvidedName;

        IConnection connection = factory.CreateConnection();
        IModel channel = connection.CreateModel();

        channel.ExchangeDeclare(rabbitMqInfoModel.ExchangeName, ExchangeType.Direct, false, false, null);
        channel.QueueDeclare(rabbitMqInfoModel.QueueName, false, false, false, null);
        channel.QueueBind(rabbitMqInfoModel.QueueName, rabbitMqInfoModel.ExchangeName, rabbitMqInfoModel.RoutingKey, null);
        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);

        Console.WriteLine("Ready.");

        consumer.Received += (sender, args) =>
        {
            var body = args.Body.ToArray();

            var messageModel = JsonSerializer.Deserialize<EmailMessageModel>(Encoding.UTF8.GetString(body));

            emailService.SendEmail(messageModel);
            Console.WriteLine("Message sent. Message: {0}", messageModel.Body);
            channel.BasicAck(args.DeliveryTag, false);
        };

        string consumerTag = channel.BasicConsume(rabbitMqInfoModel.QueueName, false, consumer);

        Console.ReadLine();
    }
}
