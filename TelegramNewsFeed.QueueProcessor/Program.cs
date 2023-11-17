using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;
using TelegramNewsFeed.Common;
using TelegramNewsFeed.QueueProcessor;

var configuration = new Configuration();

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

configurationRoot.Bind(configuration);

var telegramClient = new TelegramClient(
    configuration.ApiId,
    configuration.ApiHash,
    configuration.PhoneNumber,
    "/root/telegramsession/WTelegram_processor.session");
await telegramClient.Login();
var target = await telegramClient.GetChat(configuration.DestinationChatId);

Console.WriteLine("Target chat retrieved");

var rabbitMqClient = new RabbitMqClient("rabbitmq", "TelegramTest");
rabbitMqClient.Connect();

void ProcessMessage(BasicDeliverEventArgs eventArgs)
{
    var body = eventArgs.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    var message = JsonConvert.DeserializeObject<TelegramMessage>(json);
    Console.WriteLine($"Sending a message in chat {configuration.DestinationChatId}: {target.Title}");
    if (string.IsNullOrWhiteSpace(message.Text))
    {
        return;
    }
    telegramClient.SendMessage(target, message.Text);
}

rabbitMqClient.Consume(ProcessMessage);

Console.WriteLine("Consumer started");

Thread.Sleep(Timeout.Infinite);