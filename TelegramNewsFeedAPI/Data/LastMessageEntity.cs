using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramNewsFeed.Reader.Data
{
    public class LastMessageEntity
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int MessageId { get; set; }
    }
}
