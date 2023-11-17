using TL;
using WTelegram;

namespace TelegramNewsFeed.Common
{
    public class TelegramClient
    {
        private readonly int apiId;
        private readonly string apiHash;
        private readonly string phoneNumber;
        private readonly string sessionPath;
        private Client client;

        public TelegramClient(
            int apiId,
            string apiHash,
            string phoneNumber,
            string sessionPath)
        {
            this.apiId = apiId;
            this.apiHash = apiHash;
            this.phoneNumber = phoneNumber;
            this.sessionPath = sessionPath;
        }

        public async Task Login()
        {
            this.client = new Client(this.apiId, this.apiHash, this.sessionPath);
            Helpers.Log = (a, b) => { };

            var loginInfo = this.phoneNumber;

            while (this.client.User == null)
            {
                switch (await this.client.Login(loginInfo))
                {
                    case "verification_code": loginInfo = ""; break;
                    default: loginInfo = null; break;
                }
            }

            Console.WriteLine($"We are logged-in as {this.client.User} (id {this.client.User.id})");
        }

        public async Task<ChatBase> GetChat(int chatId)
        {
            var chats = await this.client.Messages_GetAllChats();
            return chats.chats[chatId];
        }

        public void SendMessage(ChatBase chat, string message)
            => this.client
                .SendMessageAsync(chat, message)
                .GetAwaiter()
                .GetResult();

        public async Task<IEnumerable<MessageBase>> GetChatHistory(
            ChatBase chat,
            int offsetId,
            int lastMessageId)
        {
            var history = await client.Messages_GetHistory(chat, offsetId, min_id: lastMessageId);
            return history.Messages.OrderBy(message => message.ID);
        }
    }
}
