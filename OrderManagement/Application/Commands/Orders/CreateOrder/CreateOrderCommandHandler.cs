using MediatR;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace OrderManagement.Application.Commands.Orders.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
    {
        private readonly IWriteRepository<Order> _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IWriteRepository<Order> orderRepository,
            IMapper mapper,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CustomerName))
                    return Result<OrderDto>.Failure("Customer name is required");

                var order = new Order(request.CustomerName, request.CustomerEmail);

                var orderId = await _orderRepository.AddAsync(order, cancellationToken);

                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.Id = orderId;

                _logger.LogInformation("Created order {OrderNumber} with ID {OrderId}", order.OrderNumber, orderId);

                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order");
                return Result<OrderDto>.Failure($"Failed to create order: {ex.Message}");
            }
        }
    }
}