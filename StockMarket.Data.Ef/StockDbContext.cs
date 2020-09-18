using Microsoft.EntityFrameworkCore;
using StockMarket.Core.Models;

namespace StockMarket.Data.Ef
{
    public class StockDbContext : DbContext
    {
        public virtual DbSet<Stock> Stocks { get; set; }

        public StockDbContext(DbContextOptions<StockDbContext> options /* ILoggerFactory loggerFactory */) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new StockDbConfiguration());
        }

        // need this for testing
        public virtual void SetModified(Stock stock)
        {
            Entry(stock).State = EntityState.Modified;
        }
    }
}
