using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;

namespace ClipsApiCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "ClipsAdmin API",
                    Version = "v1",
                    Description = "API methods available for access from the ClipsAdmin system",
                    Contact = new Contact { Email= "support@brack.net", Name = "ClipsAdmin Support", Url = "http://www.brack.net"}
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Set up site logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.MSSqlServer(
                    @"Password=Fruitc@kes !n the kitch3n;Persist Security Info=True;User ID=ClipsManager;Initial Catalog=ClipsSystem;Data Source=stsomewheredesign.database.windows.net;Integrated Security=False;",
                    "Serilog", LogEventLevel.Debug)
                .CreateLogger();

            var apiLogger = Log.ForContext<Startup>();
            const string applicationName = "ClipsApi";
            apiLogger.Information("{Application:l} - Logging initialized {Now} - {SourceContext}", applicationName, DateTime.Now.ToUniversalTime());

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClipsAdmin API v1");
            });
        }
    }
}
