using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace StockMarket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var contextLog = Log.ForContext("SourceContext", "Program");
            contextLog.Information($"======== Stock Marcket Api Server Starts on {DateTime.Now} ========");

            int servicePort = configuration.GetValue<int>("ServicePort", 5200);
            string urls = $"http://*:{servicePort - 1};https://*:{servicePort}";
            contextLog.Information($"======== Listening on {urls} ========");

            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseUrls(urls)
                    .UseSerilog()
                    .UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}
