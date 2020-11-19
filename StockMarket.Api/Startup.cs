using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using StockMarket.Bll;
using StockMarket.Identity;
using StockMarket.Api.Hubs;

namespace StockMarket.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning(apiVersionConfig =>
            {
                apiVersionConfig.AssumeDefaultVersionWhenUnspecified = true;
                apiVersionConfig.DefaultApiVersion = new ApiVersion(1, 0);
            });

            // services.AddAutoMapper(typeof(Startup));

            services.AddCors(options =>
            {
                IConfigurationSection CorsUrlsSection = Configuration.GetSection("CorsPorts");
                string[] CorsPorts = CorsUrlsSection.Get<string[]>();

                options.AddPolicy(name: "StockApiServerCors",
                    builder =>
                    {
                        builder
                        .SetIsOriginAllowed(host => CorsPorts.Any(p => host.Contains(p)))
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddStockServiceAndRepository(Configuration);

            services.AddAppIdentityService(Configuration)
                .AddAuthenticationConfigureJwt(Configuration);

            services.AddSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }); 
            });

            services.AddHealthChecks();

            services.AddSwaggerDocumentation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("StockApiServerCors");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerDocumentation();

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "api", pattern: "api/{controller}/{action}/{code?}");
                endpoints.MapHub<StockChangeNotification>("/notificationHub");
            });

            app.UseHealthChecks("/healthcheck");
        }
    }
}
