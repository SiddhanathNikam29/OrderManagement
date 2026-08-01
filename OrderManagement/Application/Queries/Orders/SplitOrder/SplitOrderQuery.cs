using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Queries.Orders.SplitOrder
{
    public class SplitOrderQuery : IRequest<Result<OrderSplitResultDto>>
    {
        public int OrderId { get; set; }
        public int NumberOfShares { get; set; }
    }
}