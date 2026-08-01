using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Commands.Orders.ApplyDiscount
{
    public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, Result<OrderDto>>
    {
        private readonly IWriteRepository<Order> _orderRepository;
        private readonly IOrderCalculator _orderCalculator;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplyDiscountCommandHandler> _logger;

        public ApplyDiscountCommandHandler(
            IWriteRepository<Order> orderRepository,
            IOrderCalculator orderCalculator,
            ICacheService cacheService,
            IMapper mapper,
            ILogger<ApplyDiscountCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _orderCalculator = orderCalculator;
            _cacheService = cacheService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
                if (order == null)
                    return Result<OrderDto>.Failure($"Order {request.OrderId} not found");

                if (request.DiscountValue == 0)
                {
                    order.RemoveDiscount();
                }
                else
                {
                    order.ApplyDiscount(request.DiscountType, request.DiscountValue);
                }

                var calculatedOrder = _orderCalculator.CalculateTotals(order);

                await _orderRepository.UpdateAsync(calculatedOrder, cancellationToken);

                // Invalidate cache
                await _cacheService.RemoveAsync($"order_details_{request.OrderId}", cancellationToken);
                await _cacheService.RemoveAsync($"order_summary_{request.OrderId}", cancellationToken);

                _logger.LogInformation("Applied {DiscountType} discount of {Value} to order {OrderId}",
                    request.DiscountType, request.DiscountValue, order.Id);

                var orderDto = _mapper.Map<OrderDto>(calculatedOrder);
                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply discount to order {OrderId}", request.OrderId);
                return Result<OrderDto>.Failure($"Failed to apply discount: {ex.Message}");
            }
        }
    }
}