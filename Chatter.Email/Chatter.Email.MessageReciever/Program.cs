using Chatter.Email.Core.Interfaces;
using Chatter.Email.Core.Models;
using Chatter.Email.Core.Service;
using Chatter.Email.MessageReciever.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
class Program {
    static void Main() {
        const string SenderFilename = @"..\..\..\..\Chatter.Email.MessageReciever\RabbitMQ.json";

        IEmailService emailService = new SmtpEmailService();

        var rabbitMqInfo = JsonSerializer.Deserialize<RabbitMQInfo>(File.ReadAllText(SenderFilename));

        ConnectionFactory factory = new();

        factory.Uri = new Uri(rabbitMqInfo.Uri);
        factory.ClientProvidedName = rabbitMqInfo.ClientProvidedName;

        IConnection connection = factory.CreateConnection();
        IModel channel = connection.CreateModel();

        channel.ExchangeDeclare(rabbitMqInfo.ExchangeName, ExchangeType.Direct, false, false, null);
        channel.QueueDeclare(rabbitMqInfo.QueueName, false, false, false, null);
        channel.QueueBind(rabbitMqInfo.QueueName, rabbitMqInfo.ExchangeName, rabbitMqInfo.RoutingKey, null);
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

    cancel:
        string consumerTag = channel.BasicConsume(rabbitMqInfo.QueueName, false, consumer);
        Console.ReadLine();

    askagain:
        Console.Write("Shut down component? (y/n)");
        var input = Console.ReadLine();

        if (input.ToLower() == "n")
        {
            goto cancel;
        }

        if (input.ToLower() != "y")
        {
            goto askagain;
        }

       

        channel.Close();

        connection.Close();
    }
}
