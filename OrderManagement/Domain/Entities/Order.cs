using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; private set; }
        public string CustomerName { get; private set; }
        public string CustomerEmail { get; private set; }
        public DateTime OrderDate { get; private set; }

        // Public getter, private setter
        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();

        public decimal Subtotal { get; private set; }
        public string DiscountType { get; private set; }
        public decimal? DiscountValue { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TaxableAmount { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal Total { get; private set; }
        public string Status { get; private set; }
        public int Version { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Parameterless constructor for Dapper
        public Order()
        {
            Items = new List<OrderItem>();
        }

        public Order(string customerName, string customerEmail) : this()
        {
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            OrderDate = DateTime.UtcNow;
            Status = "Active";
            Version = 1;
            UpdatedAt = DateTime.UtcNow;
            GenerateOrderNumber();
        }

        private void GenerateOrderNumber()
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }

        // PUBLIC METHOD TO SET ITEMS (USED BY DAPPER)
        public void SetItems(IEnumerable<OrderItem> items)
        {
            Items = items?.ToList() ?? new List<OrderItem>();
        }

        // PUBLIC METHOD TO CLEAR AND ADD ITEMS
        public void ClearItems()
        {
            Items.Clear();
        }

        public void AddItem(Product product, int quantity)
        {
            if (Status != "Active")
                throw new InvalidOperationException("Cannot add items to a non-active order");

            var existingItem = Items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                Items.Add(new OrderItem(this.Id, product.Id, product.Name, product.UnitPrice, quantity));
            }
        }

        public void RemoveItem(int itemId)
        {
            if (Status != "Active")
                throw new InvalidOperationException("Cannot remove items from a non-active order");

            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
                Items.Remove(item);
        }

        public void ApplyDiscount(string discountType, decimal discountValue)
        {
            if (Status != "Active")
                throw new InvalidOperationException("Cannot apply discount to a non-active order");

            if (discountValue < 0)
                throw new ArgumentException("Discount value cannot be negative");

            if (discountType == "Percentage" && discountValue > 100)
                throw new ArgumentException("Percentage discount cannot exceed 100%");

            if (discountType != "Percentage" && discountType != "Fixed")
                throw new ArgumentException("Discount type must be 'Percentage' or 'Fixed'");

            DiscountType = discountType;
            DiscountValue = discountValue;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveDiscount()
        {
            DiscountType = null;
            DiscountValue = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTotals(decimal subtotal, decimal discountAmount, decimal taxableAmount, decimal taxAmount, decimal total)
        {
            Subtotal = Math.Round(subtotal, 2);
            DiscountAmount = Math.Round(discountAmount, 2);
            TaxableAmount = Math.Round(taxableAmount, 2);
            TaxAmount = Math.Round(taxAmount, 2);
            Total = Math.Round(total, 2);
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }

        public void Complete()
        {
            if (Status != "Active")
                throw new InvalidOperationException("Only active orders can be completed");
            Status = "Completed";
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == "Completed")
                throw new InvalidOperationException("Cannot cancel a completed order");
            Status = "Cancelled";
            UpdatedAt = DateTime.UtcNow;
        }
    }
}