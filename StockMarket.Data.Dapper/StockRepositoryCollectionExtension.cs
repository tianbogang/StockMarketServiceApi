using Microsoft.Extensions.DependencyInjection;
using StockMarket.Core.Contracts;

namespace StockMarket.Data.Dapper
{
    public static class StockRepositoryCollectionExtension
    {
        public static IServiceCollection AddStockRepositoryDapper(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IStockDbContext>(_ => new StockDbContext(connectionString));
            services.AddScoped<IStockRepository, StockRepository>();
            return services;
        }
    }
}
