using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SuppaServices.Server.Configuration;

namespace SuppaServices.Server.DataAccess
{
    public class ConnectionManager : IConnectionManager
    {
        private Dictionary<string, ConnectionWrapper> _knownConnections;
        private readonly IOptions<ConnectionStrings> _connectionStrings;
        private readonly IConnectionFactory _connectionFactory;

        public ConnectionManager(IOptions<ConnectionStrings> connectionStrings, IConnectionFactory connectionFactory)
        {
            _connectionStrings = connectionStrings;
            _connectionFactory = connectionFactory;
        }
        public IDbConnectionWrapper GetConnection(string connectionName = "main", bool? transactionRequired = null)
        {
            if (_knownConnections != null)
            {
                if (_knownConnections.TryGetValue(connectionName, out var connectionWrapper))
                {
                    return connectionWrapper.Clone();
                }

                var connectionString = "";

                switch (connectionName)
                {
                    case "main":
                        connectionString = _connectionStrings.Value.Main;
                        break;
                }

                try
                {
                    var connection = _connectionFactory.CreateConnection(connectionString);
                    
                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    connectionWrapper = new ConnectionWrapper(connection, transaction, false);

                    _knownConnections.TryAdd(connectionName, connectionWrapper);

                    return connectionWrapper.Clone();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else if (transactionRequired.GetValueOrDefault(false))
            {
                throw new Exception("Required transaction was not found, make sure top level service method has a [Transaction] attribute");
            }
            else
            {
                var connectionString = "";

                switch (connectionName)
                {
                    case "main":
                        connectionString = _connectionStrings.Value.Main;
                        break;
                }

                var connection = _connectionFactory.CreateConnection(connectionString);

                connection.Open();

                return new ConnectionWrapper(connection, null, true);
            }
        }


        public void BeginTransaction()
        {
            if (_knownConnections == null)
            {
                _knownConnections = new Dictionary<string, ConnectionWrapper>();
            }
        }

        public void CommitTransaction()
        {
            if (_knownConnections != null)
            {
                foreach (var dbConnectionWrapper in _knownConnections)
                {
                    dbConnectionWrapper.Value.Transaction.Commit();
                    dbConnectionWrapper.Value.Dispose();
                    dbConnectionWrapper.Value.Connection.Close();
                    dbConnectionWrapper.Value.Connection.Dispose();
                }
            }

            _knownConnections = null;
        }

        public void RollbackTransaction()
        {
            var currentValue = Interlocked.Exchange(ref _knownConnections, null);

            if (currentValue != null)
            {
                foreach (var dbConnectionWrapper in _knownConnections)
                {
                    try
                    {
                        dbConnectionWrapper.Value.Transaction.Rollback();
                    }
                    catch (Exception e)
                    {
                        // probably want to log this
                    }
                    finally
                    {
                        dbConnectionWrapper.Value.Transaction.Dispose();
                        dbConnectionWrapper.Value.Connection.Close();
                        dbConnectionWrapper.Value.Connection.Dispose();
                    }
                }
            }
        }
    }
}
