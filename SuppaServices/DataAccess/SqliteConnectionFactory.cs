using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SuppaServices.Server.DataAccess
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection(string connectionString);
    }

    public class SqliteConnectionFactory : IConnectionFactory
    {
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}
