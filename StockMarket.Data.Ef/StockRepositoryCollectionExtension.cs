using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockMarket.Core.Contracts;

namespace StockMarket.Data.Ef
{
    public static class StockRepositoryCollectionExtension
    {
        public static IServiceCollection AddStockRepositoryEf(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<StockDbContext>(options => { options.UseSqlServer(connectionString); });
            services.AddScoped<IStockRepository, StockRepository>();
            return services;
        }
    }
}
