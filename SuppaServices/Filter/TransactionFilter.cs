using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyRpc.AspNetCore;
using SuppaServices.Server.DataAccess;

namespace SuppaServices.Server.Filter
{
    public class TransactionFilter : ICallExecuteFilter, ICallExceptionFilter
    {
        private readonly IConnectionManager _connectionManager;

        /// <summary>
        /// Filters participate in dependency injection so you can take request scoped dependencies
        /// </summary>
        public TransactionFilter(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task BeforeExecute(ICallExecutionContext context)
        {
            _connectionManager.BeginTransaction();
        }

        public async Task AfterExecute(ICallExecutionContext context)
        {
            _connectionManager.CommitTransaction();
        }

        public async Task HandleException(ICallExecutionContext context, Exception exception)
        {
            _connectionManager.RollbackTransaction();
        }
    }
}
