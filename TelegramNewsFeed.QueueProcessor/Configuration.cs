namespace TelegramNewsFeed.QueueProcessor
{
    public class Configuration
    {
        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public string PhoneNumber { get; set; }
        public int DestinationChatId { get; set; }
    }
}
