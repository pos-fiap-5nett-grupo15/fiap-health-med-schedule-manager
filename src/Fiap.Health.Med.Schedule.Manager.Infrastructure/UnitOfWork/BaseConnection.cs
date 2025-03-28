using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.UnitOfWork
{
    public class BaseConnection : IBaseConnection
    {
        private string ConnectionString { get; set; }

        private Lazy<IDbConnection> _connection;

        public IDbConnection Connection
        {
            get { return this._connection.Value; }
        }

        public BaseConnection(IConfiguration configuration)
        {
            var strconnection = configuration.GetConnectionString("DefaultConnection");


            if (string.IsNullOrEmpty(strconnection))
                throw new InvalidOperationException("DefaultConnection is not defined.");

            this.ConnectionString = strconnection;
            this._connection = new Lazy<IDbConnection>(InitializeConnection, true);
        }

        public IDbConnection InitializeConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }

        public void EnsureConnectionIsOpen()
        {
            if (this.Connection.State == ConnectionState.Broken) this.Connection.Close();

            if (this.Connection.State == ConnectionState.Closed) this.Connection.Open();
        }

        #region IDisposible support

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (this._connection.IsValueCreated) this.Connection.Dispose();
                }
                this._disposedValue = true;
            }
        }

        public void Dispose() => this.Dispose(true);
        #endregion

    }
}
