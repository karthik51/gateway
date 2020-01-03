using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System;
using System.IO;
using System.Net.Http;

namespace TMS.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
               .UseKestrel()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config
                       .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                       .AddJsonFile("appsettings.json", true, true)
                       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                       .AddJsonFile("ocelot.json")
                       .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json")
                       .AddEnvironmentVariables();
               })
               .ConfigureServices(s => {
                   s.AddOcelot().AddConsul().AddPolly();  
                   s.AddCors(options =>
                   {
                       options.AddPolicy("CorsPolicy",
                           builder => builder
                           .SetIsOriginAllowed((host) => true)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials());
                   });
               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                   //add your logging   
                  
                  
               })
               .UseIIS()
               .Configure(app =>
               {
                   app.UseCors("CorsPolicy");
                   app.UseOcelot().Wait();
               })
               .Build()
               .Run();
        }       
    }
}
