using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Queries.Products.GetProductCatalogue;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get product catalogue with filters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string category = null,
            [FromQuery] bool? isTaxable = null,
            [FromQuery] string search = null)
        {
            var query = new GetProductCatalogueQuery
            {
                Category = category,
                IsTaxable = isTaxable,
                SearchTerm = search
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(new { error = result.Error });
        }
    }
}