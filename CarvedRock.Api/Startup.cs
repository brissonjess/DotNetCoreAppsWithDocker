using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using CarvedRock.Api.Domain;
using CarvedRock.Api.Interfaces;
using CarvedRock.Api.Middleware;
using Serilog;

namespace CarvedRock.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //get configuration values from the appSettings json files
            var connectionString = Configuration.GetConnectionString("Db");
            var simpleProperty = Configuration.GetValue<string>("SimpleProperty");
            var nestedProp = Configuration.GetValue<string>("Inventory:NestedProperty");

            Log.ForContext("ConnectionString", connectionString)
                .ForContext("SimpleProperty", simpleProperty)
                .ForContext("NestedProperty", nestedProp)
                .Information("Loaded configuration!", connectionString);

            //this method is good for debugging all of your configuration settings
            //should only be used in a dev environment for finding config errors
            //warning: can expose a lot of settings that you do not want others to know
            //var dbgView = (Configuration as IConfigurationRoot).GetDebugView();
            //Log.ForContext("ConfigurationDebug:",dbgView)
            //    .Information("Config dump.");

            services.AddScoped<IProductLogic, ProductLogic>();
            services.AddScoped<IQuickOrderLogic, QuickOrderLogic>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarvedRock.Api", Version = "v1", Description = "The API for the best outdoor recreactional gear on the planet!" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CustomExceptionHandlingMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CarvedRock.Api v1"));
            }

            app.UseCustomRequestLogging();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
