using System.Data;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.UnitOfWork
{
    public interface IBaseConnection : IDisposable
    {
        IDbConnection Connection { get; }
        void EnsureConnectionIsOpen();
        IDbConnection InitializeConnection();

    }
}
