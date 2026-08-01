using Dapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Interfaces;

namespace OrderManagement.Application.Queries.Products.GetProductCatalogue
{
    public class GetProductCatalogueQueryHandler : IRequestHandler<GetProductCatalogueQuery, Result<IEnumerable<ProductDto>>>
    {
        private readonly IDapperContext _dapperContext;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetProductCatalogueQueryHandler> _logger;

        public GetProductCatalogueQueryHandler(
            IDapperContext dapperContext,
            ICacheService cacheService,
            ILogger<GetProductCatalogueQueryHandler> logger)
        {
            _dapperContext = dapperContext;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<ProductDto>>> Handle(GetProductCatalogueQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"products_cat_{request.Category ?? "all"}_tax_{request.IsTaxable?.ToString() ?? "all"}_search_{request.SearchTerm ?? "none"}";

                // Try cache first
                var cached = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey, cancellationToken);
                if (cached != null)
                {
                    _logger.LogInformation("Returning cached product catalogue");
                    return Result<IEnumerable<ProductDto>>.Success(cached);
                }

                using var connection = _dapperContext.CreateReadConnection();

                var products = await connection.QueryAsync<ProductDto>(
                    "sp_GetProductCatalogue",
                    new
                    {
                        Category = request.Category,
                        IsTaxable = request.IsTaxable,
                        SearchTerm = request.SearchTerm
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                var productList = products.ToList();

                // Cache for 10 minutes
                await _cacheService.SetAsync(cacheKey, productList, TimeSpan.FromMinutes(10), cancellationToken);

                return Result<IEnumerable<ProductDto>>.Success(productList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get product catalogue");
                return Result<IEnumerable<ProductDto>>.Failure($"Failed to get products: {ex.Message}");
            }
        }
    }
}