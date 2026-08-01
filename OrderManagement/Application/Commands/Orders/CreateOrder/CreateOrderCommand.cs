using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Commands.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<Result<OrderDto>>
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
}