using Dapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Queries.Orders.GetOrderDetails
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, Result<OrderDto>>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetOrderDetailsQueryHandler> _logger;

        public GetOrderDetailsQueryHandler(
            IDapperContext dapperContext,
            ICacheService cacheService,
            ILogger<GetOrderDetailsQueryHandler> logger)
        {
            _dapperContext = dapperContext;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"order_details_{request.OrderId}";

                // Try cache first
                var cached = await _cacheService.GetAsync<OrderDto>(cacheKey, cancellationToken);
                if (cached != null)
                {
                    _logger.LogInformation("Returning cached order details for {OrderId}", request.OrderId);
                    return Result<OrderDto>.Success(cached);
                }

                using var connection = _dapperContext.CreateReadConnection();

                using var result = await connection.QueryMultipleAsync(
                    "sp_GetOrderDetails",
                    new { OrderId = request.OrderId },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var orderHeader = await result.ReadFirstOrDefaultAsync<dynamic>();
                if (orderHeader == null)
                    return Result<OrderDto>.Failure($"Order {request.OrderId} not found");

                var orderDto = new OrderDto
                {
                    Id = orderHeader.OrderId,
                    OrderNumber = orderHeader.OrderNumber,
                    CustomerName = orderHeader.CustomerName,
                    CustomerEmail = orderHeader.CustomerEmail,
                    OrderDate = orderHeader.OrderDate,
                    Subtotal = orderHeader.Subtotal,
                    DiscountType = orderHeader.DiscountType,
                    DiscountValue = orderHeader.DiscountValue,
                    DiscountAmount = orderHeader.DiscountAmount,
                    TaxableAmount = orderHeader.TaxableAmount,
                    TaxAmount = orderHeader.TaxAmount,
                    Total = orderHeader.Total,
                    Status = orderHeader.Status,
                    Version = orderHeader.Version,
                    UpdatedAt = orderHeader.UpdatedAt,
                    Items = new List<OrderItemDto>()
                };

                var items = await result.ReadAsync<dynamic>();
                foreach (var item in items)
                {
                    orderDto.Items.Add(new OrderItemDto
                    {
                        Id = item.ItemId,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        LineTotal = item.LineTotal,
                      });
                }

                // Cache for 5 minutes
                await _cacheService.SetAsync(cacheKey, orderDto, TimeSpan.FromMinutes(5), cancellationToken);

                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order details for {OrderId}", request.OrderId);
                return Result<OrderDto>.Failure($"Failed to get order details: {ex.Message}");
            }
        }
    }
}