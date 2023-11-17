using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TelegramNewsFeed.Common;
using TelegramNewsFeed.Reader;
using TelegramNewsFeed.Reader.Data;
using TL;

var configuration = new Configuration();

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

configurationRoot.Bind(configuration);

var rabbitMqClient = new RabbitMqClient("rabbitmq", "TelegramTest");
rabbitMqClient.Connect();

var client = new TelegramClient(
    configuration.ApiId,
    configuration.ApiHash,
    configuration.PhoneNumber,
    "/root/telegramsession/WTelegram_reader.session");
await client.Login();
var peer = await client.GetChat(configuration.SourceChatId);

var lastMessageId = 0;

using var context = new TelegramDataContext();
context.Database.EnsureCreated();

while (true)
{
    var trackingRecord = context.TelegramLastMessages.FirstOrDefault(t => t.ChatId == configuration.SourceChatId);

    if (trackingRecord != null)
    {
        lastMessageId = trackingRecord.MessageId;
    }

    for (int offset_id = 0; ;)
    {
        var messages = await client.GetChatHistory(peer, offset_id, lastMessageId);
        if (!messages.Any()) break;
        foreach (var msgBase in messages)
        {
            if (msgBase is Message msg)
            {
                Console.WriteLine($"{msg.ID} {msg.message}");
                var message = new TelegramMessage { Id = msg.ID, Text = msg.message };
                var json = JsonConvert.SerializeObject(message);
                rabbitMqClient.Publish(json);
            }

            if (trackingRecord == null)
            {
                trackingRecord = new LastMessageEntity { ChatId = configuration.SourceChatId, MessageId = msgBase.ID };
                context.Add(trackingRecord);
            }
            else
            {
                trackingRecord.MessageId = msgBase.ID;
                context.Update(trackingRecord);
            }

            context.SaveChanges();
            Console.WriteLine();
        }
        offset_id = messages.Last().ID;
    }

    var waitTime = 15 * 1000;

    Console.WriteLine($"Waiting for {waitTime}ms");
    await Task.Delay(waitTime);
}

