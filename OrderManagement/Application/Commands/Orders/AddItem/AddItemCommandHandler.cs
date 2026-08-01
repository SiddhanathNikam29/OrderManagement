using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Commands.Orders.AddItem
{
    public class AddItemCommandHandler : IRequestHandler<AddItemCommand, Result<OrderDto>>
    {
        private readonly IWriteRepository<Order> _orderRepository;
        private readonly IReadRepository<Product> _productRepository;
        private readonly IOrderCalculator _orderCalculator;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddItemCommandHandler> _logger;

        public AddItemCommandHandler(
            IWriteRepository<Order> orderRepository,
            IReadRepository<Product> productRepository,
            IOrderCalculator orderCalculator,
            ICacheService cacheService,
            IMapper mapper,
            ILogger<AddItemCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _orderCalculator = orderCalculator;
            _cacheService = cacheService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> Handle(AddItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Quantity <= 0)
                    return Result<OrderDto>.Failure("Quantity must be greater than 0");

                var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
                if (order == null)
                    return Result<OrderDto>.Failure($"Order {request.OrderId} not found");

                var products = await _productRepository.GetAllAsync(cancellationToken);
                var product = products.FirstOrDefault(p => p.Id == request.ProductId);
                if (product == null)
                    return Result<OrderDto>.Failure($"Product {request.ProductId} not found");

                if (!product.IsActive)
                    return Result<OrderDto>.Failure($"Product {product.Name} is not active");

                order.AddItem(product, request.Quantity);
                var calculatedOrder = _orderCalculator.CalculateTotals(order);

                await _orderRepository.UpdateAsync(calculatedOrder, cancellationToken);

                // Invalidate cache
                await _cacheService.RemoveAsync($"order_details_{request.OrderId}", cancellationToken);
                await _cacheService.RemoveAsync($"order_summary_{request.OrderId}", cancellationToken);

                _logger.LogInformation("Added {Quantity} of {ProductName} to order {OrderId}",
                    request.Quantity, product.Name, order.Id);

                var orderDto = _mapper.Map<OrderDto>(calculatedOrder);
                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add item to order {OrderId}", request.OrderId);
                return Result<OrderDto>.Failure($"Failed to add item: {ex.Message}");
            }
        }
    }
}