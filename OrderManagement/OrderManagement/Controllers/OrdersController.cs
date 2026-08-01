using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Commands.Orders.AddItem;
using OrderManagement.Application.Commands.Orders.ApplyDiscount;
using OrderManagement.Application.Commands.Orders.CreateOrder;
using OrderManagement.Application.Commands.Orders.RemoveItem;
using OrderManagement.Application.Queries.Orders.GetAllOrders;
using OrderManagement.Application.Queries.Orders.GetOrderDetails;
using OrderManagement.Application.Queries.Orders.GetOrderSummary;
using OrderManagement.Application.Queries.Orders.SplitOrder;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }

        /// <summary>
        /// Get order details by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var query = new GetOrderDetailsQuery { OrderId = id };
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { error = result.Error });
        }

        /// <summary>
        /// Get order summary (optimized for display)
        /// </summary>
        [HttpGet("{id}/summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderSummary(int id)
        {
            var query = new GetOrderSummaryQuery { OrderId = id };
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return NotFound(new { error = result.Error });
        }

        /// <summary>
        /// Get all orders with pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllOrdersQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }

        /// <summary>
        /// Add item to order
        /// </summary>
        [HttpPost("{orderId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddItemToOrder(int orderId, [FromBody] AddItemCommand command)
        {
            if (orderId != command.OrderId)
                return BadRequest(new { error = "Order ID mismatch" });

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }

        /// <summary>
        /// Remove item from order
        /// </summary>
        [HttpDelete("{orderId}/items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveItemFromOrder(int orderId, int itemId)
        {
            var command = new RemoveItemCommand { OrderId = orderId, ItemId = itemId };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }

        /// <summary>
        /// Apply discount to order
        /// </summary>
        [HttpPatch("{orderId}/discount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApplyDiscount(int orderId, [FromBody] ApplyDiscountCommand command)
        {
            if (orderId != command.OrderId)
                return BadRequest(new { error = "Order ID mismatch" });

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }

        /// <summary>
        /// Split order into equal shares (Part Two)
        /// </summary>
        [HttpPost("{orderId}/split")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SplitOrder(int orderId, [FromBody] SplitOrderQuery query)
        {
            if (orderId != query.OrderId)
                return BadRequest(new { error = "Order ID mismatch" });

            var result = await _mediator.Send(query);
            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }
    }
}