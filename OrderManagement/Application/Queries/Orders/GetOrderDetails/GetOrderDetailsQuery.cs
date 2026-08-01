using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Queries.Orders.GetOrderDetails
{
    public class GetOrderDetailsQuery : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }
    }
}