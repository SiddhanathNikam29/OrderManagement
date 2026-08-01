using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; private set; }
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal { get; private set; }
        // ✅ Remove IsTaxable - it comes from Product
        public DateTime CreatedAt { get; private set; }

        private OrderItem() { }

        public OrderItem(int orderId, int productId, string productName, decimal unitPrice, int quantity)
        {
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            UnitPrice = Math.Round(unitPrice, 2);
            Quantity = quantity;
            LineTotal = Math.Round(UnitPrice * quantity, 2);
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");
            Quantity = quantity;
            LineTotal = Math.Round(UnitPrice * quantity, 2);
        }
    }
}