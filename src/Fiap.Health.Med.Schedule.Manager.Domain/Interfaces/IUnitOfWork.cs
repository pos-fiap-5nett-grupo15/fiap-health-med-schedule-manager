using System.Transactions;

namespace Fiap.Health.Med.Schedule.Manager.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IScheduleRepository ScheduleRepository { get; }

        ITransaction BeginTransaction();

        ITransaction BeginTransaction(TransactionOptions transactionOptions);

    }
}
