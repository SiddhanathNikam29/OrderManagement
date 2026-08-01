using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Commands.Orders.ApplyDiscount
{
    public class ApplyDiscountCommand : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }
        public string DiscountType { get; set; } // "Percentage" or "Fixed"
        public decimal DiscountValue { get; set; }
    }
}