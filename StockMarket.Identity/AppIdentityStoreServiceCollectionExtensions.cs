using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace StockMarket.Identity
{
    public static class AppIdentityStoreServiceCollectionExtensions
    {
        public static IServiceCollection AddAppIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("StockDbConnection");
            services.AddDbContext<AppIdentityDbContext>(options => { options.UseSqlServer(connectionString); });

            services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            return services;
        }
    }
}
