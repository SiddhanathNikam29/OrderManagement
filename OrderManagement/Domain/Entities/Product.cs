using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal UnitPrice { get; private set; }
        public bool IsTaxable { get; private set; }
        public string Category { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        private Product() { }

        public Product(string name, string description, decimal unitPrice, bool isTaxable, string category)
        {
            Name = name;
            Description = description;
            UnitPrice = Math.Round(unitPrice, 2);
            IsTaxable = isTaxable;
            Category = category;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Update(string name, string description, decimal unitPrice, bool isTaxable, string category)
        {
            Name = name;
            Description = description;
            UnitPrice = Math.Round(unitPrice, 2);
            IsTaxable = isTaxable;
            Category = category;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}