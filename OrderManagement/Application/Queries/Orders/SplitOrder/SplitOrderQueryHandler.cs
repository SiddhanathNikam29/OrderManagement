using Dapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Interfaces;
using System.Text.Json;

namespace OrderManagement.Application.Queries.Orders.SplitOrder
{
    public class SplitOrderQueryHandler : IRequestHandler<SplitOrderQuery, Result<OrderSplitResultDto>>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ILogger<SplitOrderQueryHandler> _logger;

        public SplitOrderQueryHandler(
            IDapperContext dapperContext,
            ILogger<SplitOrderQueryHandler> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<Result<OrderSplitResultDto>> Handle(SplitOrderQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.NumberOfShares < 2)
                    return Result<OrderSplitResultDto>.Failure("Number of shares must be at least 2");

                using var connection = _dapperContext.CreateReadConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@OrderId", request.OrderId);
                parameters.Add("@NumberOfShares", request.NumberOfShares);
                parameters.Add("@SplitData", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: -1);

                await connection.ExecuteAsync(
                    "sp_SplitOrder",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var splitDataJson = parameters.Get<string>("@SplitData");

                if (string.IsNullOrEmpty(splitDataJson))
                    return Result<OrderSplitResultDto>.Failure($"Order {request.OrderId} not found or not active");

                using var document = JsonDocument.Parse(splitDataJson);
                var root = document.RootElement;

                var result = new OrderSplitResultDto
                {
                    OrderId = request.OrderId,
                    TotalAmount = root.GetProperty("TotalAmount").GetDecimal(),
                    NumberOfShares = root.GetProperty("NumberOfShares").GetInt32(),
                    Shares = new List<OrderShareDto>()
                };

                var sharesArray = root.GetProperty("Shares").EnumerateArray();
                foreach (var share in sharesArray)
                {
                    result.Shares.Add(new OrderShareDto
                    {
                        ShareNumber = share.GetProperty("ShareNumber").GetInt32(),
                        Amount = share.GetProperty("Amount").GetDecimal()
                    });
                }

                // Get order number
                var orderInfo = await connection.QueryFirstOrDefaultAsync(
                    "SELECT OrderNumber FROM Orders WHERE Id = @OrderId AND Status = 'Active'",
                    new { OrderId = request.OrderId }
                );

                if (orderInfo != null)
                {
                    result.OrderNumber = orderInfo.OrderNumber;
                }

                _logger.LogInformation("Split order {OrderId} into {Shares} shares", request.OrderId, request.NumberOfShares);

                return Result<OrderSplitResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to split order {OrderId}", request.OrderId);
                return Result<OrderSplitResultDto>.Failure($"Failed to split order: {ex.Message}");
            }
        }
    }
}