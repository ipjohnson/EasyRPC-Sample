using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SuppaServices.Server.DataAccess
{
    public interface IDbConnectionWrapper : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }
    }
}
