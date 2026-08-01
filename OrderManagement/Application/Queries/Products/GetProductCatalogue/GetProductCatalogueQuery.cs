using MediatR;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Queries.Products.GetProductCatalogue
{
    public class GetProductCatalogueQuery : IRequest<Result<IEnumerable<ProductDto>>>
    {
        public string Category { get; set; }
        public bool? IsTaxable { get; set; }
        public string SearchTerm { get; set; }
    }
}