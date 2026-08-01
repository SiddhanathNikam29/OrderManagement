using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Queries.Orders.GetOrderSummary
{
    public class GetOrderSummaryQuery : IRequest<Result<OrderSummaryDto>>
    {
        public int OrderId { get; set; }
    }
}