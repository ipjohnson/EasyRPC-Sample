using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SuppaServices.Server.DataAccess
{
    public class ConnectionWrapper : IDbConnectionWrapper
    {
        private bool _shouldDispose;

        public ConnectionWrapper(IDbConnection connection, IDbTransaction transaction, bool shouldDispose)
        {
            Connection = connection;
            Transaction = transaction;
            _shouldDispose = shouldDispose;
        }

        public IDbConnection Connection { get; }

        public IDbTransaction Transaction { get; }

        public void Dispose()
        {
            if (_shouldDispose)
            {
                _shouldDispose = false;
                Transaction?.Dispose();
                Connection.Dispose();
            }
        }

        public ConnectionWrapper Clone()
        {
            return new ConnectionWrapper(Connection, Transaction, _shouldDispose);
        }
    }
}
