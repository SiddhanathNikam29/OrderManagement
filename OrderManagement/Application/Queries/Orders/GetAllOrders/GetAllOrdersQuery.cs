using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Queries.Orders.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<Result<PagedResult<OrderSummaryDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}