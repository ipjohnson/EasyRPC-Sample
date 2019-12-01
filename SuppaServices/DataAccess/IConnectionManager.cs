using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuppaServices.Server.DataAccess
{
    public interface IConnectionManager
    {
        IDbConnectionWrapper GetConnection(string connectionName = "main", bool? transactionRequired = null);

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
