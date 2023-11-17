using Microsoft.EntityFrameworkCore;

namespace TelegramNewsFeed.Reader.Data
{
    public class TelegramDataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("");
        }

        public DbSet<LastMessageEntity> TelegramLastMessages { get; set; }
    }
}
