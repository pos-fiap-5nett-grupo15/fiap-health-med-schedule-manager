using System.Transactions;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.UnitOfWork
{
    public class Transaction : ITransaction
    {
        private readonly TransactionScope _transactionScope;

        public Transaction()
        {
            this._transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        public Transaction(TransactionOptions scopeOption)
        {
            this._transactionScope =
                new TransactionScope(TransactionScopeOption.Required, scopeOption, TransactionScopeAsyncFlowOption.Enabled);
        }

        public void Commit()
        {
            this._transactionScope.Complete();
        }

        public void Rollback()
        {
            this.Dispose();
        }


        #region IDisposible Support
        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) this._transactionScope.Dispose();
                this._disposedValue = true;
            }
        }

        public void Dispose() => this.Dispose(true);


        #endregion
    }
}
