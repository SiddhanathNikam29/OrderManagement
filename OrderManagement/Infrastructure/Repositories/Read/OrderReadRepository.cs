using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Infrastructure.Data;
using System.Data;

namespace OrderManagement.Infrastructure.Repositories.Read
{
    public class OrderReadRepository : IReadRepository<Order>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ILogger<OrderReadRepository> _logger;

        public OrderReadRepository(IDapperContext dapperContext, ILogger<OrderReadRepository> logger)
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
                    // ✅ Query WITHOUT IsTaxable
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
                    // ✅ Query WITHOUT IsTaxable
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
    }
}