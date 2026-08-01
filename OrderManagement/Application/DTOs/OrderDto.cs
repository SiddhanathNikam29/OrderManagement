namespace OrderManagement.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        // ✅ IsTaxable removed - comes from Product
    }

    /// <summary>
    /// Product Data Transfer Object
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsTaxable { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Computed property - derived from IsTaxable
        /// </summary>
        public string TaxStatus => IsTaxable ? "Taxable" : "Zero-Rated";
    }
}