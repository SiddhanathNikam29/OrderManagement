using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Data;
using System.Data;

namespace OrderManagement.Infrastructure.Repositories.Write
{
    public class ProductWriteRepository : IWriteRepository<Product>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ILogger<ProductWriteRepository> _logger;

        public ProductWriteRepository(IDapperContext dapperContext, ILogger<ProductWriteRepository> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateReadConnection();

            try
            {
                return await connection.QueryFirstOrDefaultAsync<Product>(
                    "SELECT * FROM Products WHERE Id = @Id AND IsActive = 1",
                    new { Id = id }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateReadConnection();

            try
            {
                return await connection.QueryAsync<Product>(
                    "SELECT * FROM Products WHERE IsActive = 1 ORDER BY Category, Name"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                throw;
            }
        }

        public async Task<int> AddAsync(Product entity, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateWriteConnection();

            try
            {
                var sql = @"INSERT INTO Products (Name, Description, UnitPrice, IsTaxable, Category, CreatedAt, IsActive)
                            VALUES (@Name, @Description, @UnitPrice, @IsTaxable, @Category, @CreatedAt, @IsActive);
                            SELECT CAST(SCOPE_IDENTITY() as int)";

                var id = await connection.QuerySingleAsync<int>(sql, new
                {
                    entity.Name,
                    entity.Description,
                    entity.UnitPrice,
                    entity.IsTaxable,
                    entity.Category,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });

                // Set the Id on the entity
                var propertyInfo = typeof(Product).GetProperty("Id");
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(entity, id);
                }

                _logger.LogInformation("Created product {ProductName} with ID {ProductId}", entity.Name, id);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                throw;
            }
        }

        public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateWriteConnection();

            try
            {
                var sql = @"UPDATE Products SET 
                                Name = @Name,
                                Description = @Description,
                                UnitPrice = @UnitPrice,
                                IsTaxable = @IsTaxable,
                                Category = @Category
                            WHERE Id = @Id";

                await connection.ExecuteAsync(sql, new
                {
                    entity.Id,
                    entity.Name,
                    entity.Description,
                    entity.UnitPrice,
                    entity.IsTaxable,
                    entity.Category
                });

                _logger.LogInformation("Updated product {ProductId}", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", entity.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateWriteConnection();

            try
            {
                await connection.ExecuteAsync(
                    "UPDATE Products SET IsActive = 0 WHERE Id = @Id",
                    new { Id = id }
                );

                _logger.LogInformation("Deleted product {ProductId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                throw;
            }
        }
    }
}