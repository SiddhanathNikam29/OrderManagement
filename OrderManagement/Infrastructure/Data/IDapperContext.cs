using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public interface IDapperContext
    {
        IDbConnection CreateReadConnection();
        IDbConnection CreateWriteConnection();
    }
}
