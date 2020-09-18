using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockMarket.Core.Contracts;
using StockMarket.Data.Ef;
using StockMarket.Data.Dapper;
using StockMarket.Data.MongoDB;

namespace StockMarket.Bll
{
    public static class StockServiceCollectionExtension
    {
        public static IServiceCollection AddStockServiceAndRepository(this IServiceCollection services, IConfiguration configuration)
        {
            string DbProvider = configuration["SystemConfig:DatabaseProvider"];
            switch(DbProvider)
            {
                case "SqlServerEf":
                    {
                        string connectionString = configuration.GetConnectionString("StockDbConnection");
                        services.AddStockRepositoryEf(connectionString);
                    }
                    break;

                case "SqlServerDapper":
                    {
                        string connectionString = configuration.GetConnectionString("StockDbConnection");
                        services.AddStockRepositoryDapper(connectionString);
                    }
                    break;

                case "MongoDB":
                case "MongoDb":
                    {
                        IConfiguration configSection = configuration.GetSection("StockMongoDbSettings");
                        services.AddStockRepositoryMongoDb(configSection);
                    }
                    break;
            }

            services.AddScoped<IStockService, StockService>();
            return services;
        }
    }
}
