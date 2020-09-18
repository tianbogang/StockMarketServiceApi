using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using StockMarket.Core.Contracts;

namespace StockMarket.Data.MongoDB
{
    public static class StockRepositoryCollectionExtension
    {
        public static IServiceCollection AddStockRepositoryMongoDb(this IServiceCollection services, IConfiguration configSection)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            services.Configure<StockMongoDbSettings>(configSection);
            services.AddSingleton<IStockMongoDbSettings>(sp => sp.GetRequiredService<IOptions<StockMongoDbSettings>>().Value);

            services.AddSingleton<IStockDbContext, StockDbContext>();
            services.AddScoped<IStockRepository, StockRepository>();
            return services;
        }
    }
}
