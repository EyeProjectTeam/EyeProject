using EyeProtect.Manage;
using Serilog.Events;
using Serilog;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using NLog;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;

//public class Program
//{
//    public async static Task<int> Main(string[] args)
//    {
//        try
//        {
//            var builder = WebApplication.CreateBuilder(args);
//            builder.Host
//                .ConfigureAppConfiguration((context, config) =>
//                {
//                    var env = context.HostingEnvironment.EnvironmentName;
//                    // Õ¾µãÅäÖÃ
//                    config
//                        .AddJsonFile("host.json", true, true)
//                        .AddJsonFile("appsettings.json", true, true)
//                        .AddJsonFile($"appsettings.{env}.json", true, true)
//                        .AddEnvironmentVariables();
//                    if (args != null) config.AddCommandLine(args);
//                })
//                .ConfigureLogging((context, logging) =>
//                {
//                    var config = context.Configuration;
//                    logging.AddNLog();
//                    logging.AddDebug();
//                    logging.AddEventSourceLogger();

//                    logging.AddConfiguration(config.GetSection("Logging"));
//                    LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
//                })
//                .UseNLog()
//                .UseAutofac();
//            await builder.AddApplicationAsync<EyeProtectManagerApiModule>();
//            var app = builder.Build();
//            await app.InitializeApplicationAsync();

//            await app.RunAsync();
//            return 0;
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"{ex.ToString()}-{ex.StackTrace}");
//            return 1;
//        }
//        finally
//        {
//            Log.CloseAndFlush();
//        }
//    }
//}
namespace EyeProtect.Manage
{
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                var build = new HostBuilder()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureHostConfiguration(config =>
                    {
                        config.AddEnvironmentVariables("DOTNET_");
                        if (args != null) config.AddCommandLine(args);
                    })
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        var env = context.HostingEnvironment.EnvironmentName;
                        // Õ¾µãÅäÖÃ
                        config
                            //.AddJsonFile("host.json", true, true)
                            .AddJsonFile("appsettings.json", true, true)
                            .AddJsonFile($"appsettings.{env}.json", true, true)
                            .AddEnvironmentVariables();
                        if (args != null) config.AddCommandLine(args);
                    })
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.AddDebug();
                        logging.AddEventSourceLogger();

                        var config = context.Configuration;
                        logging.AddConfiguration(config.GetSection("Logging"));
                        LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
                    })
                    .UseNLog()
                    .UseAutofac()
                    .ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>());

                await build.Build().RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.ToString()}-{ex.StackTrace}");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}