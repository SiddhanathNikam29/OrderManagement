namespace OrderManagement.Application.DTOs
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemCount { get; set; }
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
        public decimal DiscountDisplayValue { get; set; }
        public string DiscountSymbol { get; set; }
    }
}