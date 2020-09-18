using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockMarket.Core.Models;

namespace StockMarket.Data.Ef
{
    internal class StockDbConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder
                .HasKey(i => i.Code);

            builder
                .ToTable("Stocks");
        }
    }
}
