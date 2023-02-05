using EyeProtect.Manager;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();

    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel((context, options) => { options.Limits.MaxRequestBodySize = 1024 * 50; });
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.AddDebug();
                logging.AddEventSourceLogger();
            }).UseAutofac();
}