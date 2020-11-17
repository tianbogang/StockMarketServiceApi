using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace StockMarket.Api
{
    public class Program
    {
        private static int servicePort;

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            servicePort = configuration.GetValue<int>("ServicePort", 5200);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var contextLog = Log.ForContext("SourceContext", "Program");
            contextLog.Information($"======== Stock Marcket Api Server Starts on {DateTime.Now} ========");

            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseUrls($"http://*:{servicePort - 1};https://*:{servicePort}")
                    .UseSerilog()
                    .UseStartup<Startup>();
                });
    }
}
