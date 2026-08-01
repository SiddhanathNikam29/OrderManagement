using Dapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Queries.Orders.GetOrderSummary
{
    public class GetOrderSummaryQueryHandler : IRequestHandler<GetOrderSummaryQuery, Result<OrderSummaryDto>>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetOrderSummaryQueryHandler> _logger;

        public GetOrderSummaryQueryHandler(
            IDapperContext dapperContext,
            ICacheService cacheService,
            ILogger<GetOrderSummaryQueryHandler> logger)
        {
            _dapperContext = dapperContext;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<OrderSummaryDto>> Handle(GetOrderSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"order_summary_{request.OrderId}";

                // Try cache first
                var cached = await _cacheService.GetAsync<OrderSummaryDto>(cacheKey, cancellationToken);
                if (cached != null)
                {
                    _logger.LogInformation("Returning cached order summary for {OrderId}", request.OrderId);
                    return Result<OrderSummaryDto>.Success(cached);
                }

                using var connection = _dapperContext.CreateReadConnection();

                var summary = await connection.QueryFirstOrDefaultAsync<OrderSummaryDto>(
                    "sp_GetOrderSummary",
                    new { OrderId = request.OrderId },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (summary == null)
                    return Result<OrderSummaryDto>.Failure($"Order {request.OrderId} not found");

                // Cache for 5 minutes
                await _cacheService.SetAsync(cacheKey, summary, TimeSpan.FromMinutes(5), cancellationToken);

                return Result<OrderSummaryDto>.Success(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get order summary for {OrderId}", request.OrderId);
                return Result<OrderSummaryDto>.Failure($"Failed to get summary: {ex.Message}");
            }
        }
    }
}