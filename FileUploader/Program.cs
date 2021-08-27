using FileUploader.Helpers;
using FileUploader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine(@"Передано неверное количество аргументов");
                return;
            }

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            var host = CreateHostBuilder(args).Build();
            var svc = ActivatorUtilities.CreateInstance<FileUploadService>(host.Services);
            svc.Run();

            Console.ReadKey();
        }

        static void BuildConfig(IConfigurationBuilder builder) => 
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ArgsOptions>(opt => {
                        opt.OutputPath = args[0];
                        opt.InputPath = args[1];
                    });
                    services.AddHttpClient();
                    services.AddTransient<IYaDiskPathParameterService, YaDiskPathParameterService>();
                    services.AddTransient<IFileUploadService, FileUploadService>();
                });
    }
}
