using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure.Repositories.Read
{
    public class ProductReadRepository : IReadRepository<Product>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ILogger<ProductReadRepository> _logger;

        public ProductReadRepository(IDapperContext dapperContext, ILogger<ProductReadRepository> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateReadConnection();

            return await connection.QueryFirstOrDefaultAsync<Product>(
                "SELECT * FROM Products WHERE Id = @Id AND IsActive = 1",
                new { Id = id }
            );
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateReadConnection();

            return await connection.QueryAsync<Product>(
                "SELECT * FROM Products WHERE IsActive = 1 ORDER BY Category, Name"
            );
        }
    }
}