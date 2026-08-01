using Dapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Infrastructure.Data;


namespace OrderManagement.Application.Queries.Orders.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PagedResult<OrderSummaryDto>>>
    {
        private readonly DapperContext _dapperContext;
        private readonly ILogger<GetAllOrdersQueryHandler> _logger;

        public GetAllOrdersQueryHandler(
            IDapperContext dapperContext,
            ILogger<GetAllOrdersQueryHandler> logger)
        {
            _dapperContext = (DapperContext?)dapperContext;
            _logger = logger;
        }

        public async Task<Result<PagedResult<OrderSummaryDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = _dapperContext.CreateReadConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Page", request.Page);
                parameters.Add("@PageSize", request.PageSize);
                parameters.Add("@TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                var orders = await connection.QueryAsync<OrderSummaryDto>(
                    "sp_GetAllOrders",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var totalCount = parameters.Get<int>("@TotalCount");

                var result = new PagedResult<OrderSummaryDto>
                {
                    Items = orders.ToList(),
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                };

                return Result<PagedResult<OrderSummaryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all orders");
                return Result<PagedResult<OrderSummaryDto>>.Failure($"Failed to get orders: {ex.Message}");
            }
        }
    }
}