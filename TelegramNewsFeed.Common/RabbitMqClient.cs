using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace TelegramNewsFeed.Common
{
    public class RabbitMqClient
    {
        private readonly string hostName;
        private readonly string queueName;
        private IModel channel;

        public RabbitMqClient(string hostName, string queueName)
        {
            this.hostName = hostName;
            this.queueName = queueName;
        }

        public void Connect()
        {
            var factory = new ConnectionFactory { HostName = this.hostName };
            var connection = factory.CreateConnection();
            this.channel = connection.CreateModel();

            this.channel.QueueDeclare(queue: this.queueName,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
        }

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            this.channel.BasicPublish(exchange: string.Empty,
                                 routingKey: this.queueName,
                                 basicProperties: null,
                                 body: body);
        }

        public void Consume(Action<BasicDeliverEventArgs> action)
        {
            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (model, ea) => action(ea);
            this.channel.BasicConsume(queue: this.queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
