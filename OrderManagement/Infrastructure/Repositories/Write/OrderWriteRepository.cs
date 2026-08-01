using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Data;
using System.Data;

namespace OrderManagement.Infrastructure.Repositories.Write
{
    public class OrderWriteRepository : IWriteRepository<Order>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ILogger<OrderWriteRepository> _logger;

        public OrderWriteRepository(IDapperContext dapperContext, ILogger<OrderWriteRepository> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<Order> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateReadConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var order = await connection.QueryFirstOrDefaultAsync<Order>(
                    "SELECT * FROM Orders WHERE Id = @Id AND Status != 'Deleted'",
                    new { Id = id }
                );

                if (order != null)
                {
                    var items = await connection.QueryAsync<OrderItem>(
                        "SELECT * FROM OrderItems WHERE OrderId = @OrderId",
                        new { OrderId = id }
                    );
                    order.SetItems(items);
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order by ID {OrderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateReadConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var orders = await connection.QueryAsync<Order>(
                    "SELECT * FROM Orders WHERE Status != 'Deleted' ORDER BY OrderDate DESC"
                );

                foreach (var order in orders)
                {
                    var items = await connection.QueryAsync<OrderItem>(
                        "SELECT * FROM OrderItems WHERE OrderId = @OrderId",
                        new { OrderId = order.Id }
                    );
                    order.SetItems(items);
                }

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        public async Task<int> AddAsync(Order entity, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateWriteConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var parameters = new DynamicParameters();
                parameters.Add("@CustomerName", entity.CustomerName);
                parameters.Add("@CustomerEmail", entity.CustomerEmail);
                parameters.Add("@OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@OrderNumber", dbType: DbType.String, direction: ParameterDirection.Output, size: 50);

                await connection.ExecuteAsync(
                    "sp_CreateOrder",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var orderId = parameters.Get<int>("@OrderId");
                var orderNumber = parameters.Get<string>("@OrderNumber");

                SetPropertyValue(entity, "Id", orderId);
                SetPropertyValue(entity, "OrderNumber", orderNumber);

                _logger.LogInformation("Created order {OrderNumber} with ID {OrderId}", orderNumber, orderId);

                return orderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order");
                throw;
            }
        }

        public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateWriteConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    // Update order header with Version increment
                    var updateOrderSql = @"
                        UPDATE Orders SET 
                            Subtotal = @Subtotal,
                            DiscountType = @DiscountType,
                            DiscountValue = @DiscountValue,
                            DiscountAmount = @DiscountAmount,
                            TaxableAmount = @TaxableAmount,
                            TaxAmount = @TaxAmount,
                            Total = @Total,
                            Status = @Status,
                            Version = @Version + 1,
                            UpdatedAt = GETUTCDATE()
                        WHERE Id = @Id";

                    await connection.ExecuteAsync(
                        updateOrderSql,
                        new
                        {
                            entity.Id,
                            entity.Subtotal,
                            entity.DiscountType,
                            entity.DiscountValue,
                            entity.DiscountAmount,
                            entity.TaxableAmount,
                            entity.TaxAmount,
                            entity.Total,
                            entity.Status,
                            entity.Version
                        },
                        transaction: transaction
                    );

                    // ✅ FIX: Delete all existing items and re-insert (simpler approach)
                    await connection.ExecuteAsync(
                        "DELETE FROM OrderItems WHERE OrderId = @OrderId",
                        new { OrderId = entity.Id },
                        transaction: transaction
                    );

                    // ✅ FIX: Insert all items WITHOUT IsTaxable
                    if (entity.Items.Any())
                    {
                        var insertSql = @"
                            INSERT INTO OrderItems 
                                (OrderId, ProductId, ProductName, UnitPrice, Quantity, LineTotal, CreatedAt)
                            VALUES
                                (@OrderId, @ProductId, @ProductName, @UnitPrice, @Quantity, @LineTotal, @CreatedAt)";

                        await connection.ExecuteAsync(
                            insertSql,
                            entity.Items.Select(item => new
                            {
                                item.OrderId,
                                item.ProductId,
                                item.ProductName,
                                item.UnitPrice,
                                item.Quantity,
                                item.LineTotal,
                                CreatedAt = DateTime.UtcNow
                            }),
                            transaction: transaction
                        );
                    }

                    transaction.Commit();
                    _logger.LogInformation("Updated order {OrderId} successfully", entity.Id);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Transaction rolled back for order {OrderId}", entity.Id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId}", entity.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            using var connection = _dapperContext.CreateWriteConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                await connection.ExecuteAsync(
                    "UPDATE Orders SET Status = 'Deleted', UpdatedAt = GETUTCDATE() WHERE Id = @Id",
                    new { Id = id }
                );

                _logger.LogInformation("Deleted order {OrderId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                throw;
            }
        }

        private void SetPropertyValue<T>(object obj, string propertyName, T value)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
        }
    }
}