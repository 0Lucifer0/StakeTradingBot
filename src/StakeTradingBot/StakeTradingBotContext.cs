using Microsoft.EntityFrameworkCore;

namespace StakeTradingBot
{
    public class StakeTradingBotContext : DbContext
    {
        public StakeTradingBotContext(DbContextOptions<StakeTradingBotContext> options)
            : base(options)
        { }

        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./StakeTradingBot.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}