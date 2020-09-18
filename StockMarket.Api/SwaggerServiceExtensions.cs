using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace StockMarket.Api
{
    public static class SwaggerServiceExtensions
    {
        // Register the Swagger generator, defining 1 or more Swagger documents
        // Config Authorization
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Stock Market Service API",
                    Version = "v1",
                    Description = "Stock market service API in Asp.net Core"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

                OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                };
                config.AddSecurityDefinition("Bearer", openApiSecurityScheme);

                OpenApiReference apiReference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                };
                OpenApiSecurityScheme apiSecurityScheme = new OpenApiSecurityScheme
                {
                    Reference = apiReference
                };
                OpenApiSecurityRequirement apiSecurityRequirement = new OpenApiSecurityRequirement
                {
                    { apiSecurityScheme, new string[] { } }
                };
                config.AddSecurityRequirement(apiSecurityRequirement);
            });

            return services;
        }

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Market Service API v1");

                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            return app;
        }
    }
}
