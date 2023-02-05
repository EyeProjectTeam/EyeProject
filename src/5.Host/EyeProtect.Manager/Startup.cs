namespace EyeProtect.Manager
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<EyeProtectManagerApiModule>();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.InitializeApplication();
        }
    }
}
