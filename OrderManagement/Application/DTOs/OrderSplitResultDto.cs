namespace OrderManagement.Application.DTOs
{
    public class OrderSplitResultDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public int NumberOfShares { get; set; }
        public List<OrderShareDto> Shares { get; set; }
    }

    public class OrderShareDto
    {
        public int ShareNumber { get; set; }
        public decimal Amount { get; set; }
    }
}