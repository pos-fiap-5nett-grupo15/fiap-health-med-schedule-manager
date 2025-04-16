namespace Fiap.Health.Med.Schedule.Manager.Domain.Interfaces
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
