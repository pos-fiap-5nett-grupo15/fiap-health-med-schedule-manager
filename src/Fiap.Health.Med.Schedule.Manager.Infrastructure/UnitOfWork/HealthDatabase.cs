using Microsoft.Extensions.Configuration;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.UnitOfWork
{
    public class HealthDatabase : BaseConnection, IHealthDatabase
    {
        public HealthDatabase(IConfiguration configuration) : base(configuration) { }
    }
}
