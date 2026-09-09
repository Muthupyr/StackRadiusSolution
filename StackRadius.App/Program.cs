using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql; // Make sure to install the Npgsql NuGet package
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
namespace StackRadius
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Register factory first
            // Include this line as early as possible in your application lifecycle (e.g., at the start of Main or inside WebApplication.CreateBuilder)
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

                //  The following added for log4net core.
                //.ConfigureLogging(builder =>
                //{
                //    builder.AddLog4Net("log4net.config");
                //});
    }
}
