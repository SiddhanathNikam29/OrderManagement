using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Infrastructure.Data;

namespace OrderManagement.Infrastructure.Data
{
   
    public class DapperContext : IDapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateReadConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IDbConnection CreateWriteConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}