using Fiap.Health.Med.Schedule.Manager.CrossCutting;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace Fiap.Health.Med.Schedule.Manager.Api;

internal class Startup
{
    public IConfiguration Configuration { get; set; }
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Hackaton", Version = "v1" });
        });
        services.AddEndpointsApiExplorer();
        services.AddControllers();
        services.AddDataServices();
        services.AddServices();
        services.AddRabbitMqService();

    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Hackathon v1");
            });
        }

        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
            {
                Predicate = _ => true
            });
            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
            {
                Predicate = check => check.Tags.Contains("ready")
            });
        });
        app.UseHttpsRedirection();
    }
}