using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
                IConfigurationSection CorsUrlsSection = Configuration.GetSection("CorsUrls");
                string[] CorsUrls = CorsUrlsSection.Get<string[]>();

                options.AddPolicy(name: "StockApiServerCors",
                    builder =>
                    {
                        builder
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(CorsUrls);
                    });
            });

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddStockServiceAndRepository(Configuration);

            services.AddAppIdentityService(Configuration);

            ConfigureAddAuthenticationJwt(services);

            services.AddSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }); 
            });

            services.AddHealthChecks();

            services.AddSwaggerDocumentation();
        }

        private void ConfigureAddAuthenticationJwt(IServiceCollection services)
        {
            var jwtSection = Configuration.GetSection("JwtBearerTokenSettings");
            services.Configure<JwtBearerTokenSettings>(jwtSection);
            var jwtBearerTokenSettings = jwtSection.Get<JwtBearerTokenSettings>();
            var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtBearerTokenSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtBearerTokenSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
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
