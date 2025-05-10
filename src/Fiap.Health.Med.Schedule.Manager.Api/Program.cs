using Fiap.Health.Med.Schedule.Manager.CrossCutting;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;

namespace Fiap.Health.Med.Schedule.Manager.Api;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var producerSettings = new ProducerSettings();
        builder.Configuration.GetSection("ProducerSettings").Bind(producerSettings);

        builder.Services.AddSingleton(producerSettings);
        builder.Services.AddSingleton<IProducerSettings>(producerSettings);

        //builder.Services.AddSingleton(new RabbitMqConnector(consumerSettings));

        builder.Services.AddHealthChecks().AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), tags: new[] { "ready" });
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        builder.Services.Migrations(builder.Configuration);

        var app = builder.Build();
        startup.Configure(app, app.Environment);
        app.Run();
    }
}