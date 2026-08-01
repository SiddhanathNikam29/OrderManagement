using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Commands.Orders.RemoveItem
{
    public class RemoveItemCommand : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
    }
}