using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyRpc.AspNetCore;

namespace SuppaServices.Server.Filter
{
    public class TransactionFilter : ICallExecuteFilter, ICallExceptionFilter
    {
        /// <summary>
        /// Filters participate in dependency injection so you can take request scoped dependencies
        /// </summary>
        public TransactionFilter()
        {

        }

        public async Task BeforeExecute(ICallExecutionContext context)
        {
            // open transaction
        }

        public async Task AfterExecute(ICallExecutionContext context)
        {
            // commit transaction
        }

        public async Task HandleException(ICallExecutionContext context, Exception exception)
        {
            // rollback transaction
        }
    }
}
