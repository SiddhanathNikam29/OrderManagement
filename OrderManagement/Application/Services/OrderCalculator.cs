using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Services
{
    public class OrderCalculator : IOrderCalculator
    {
        private const decimal TAX_RATE = 0.20m;

        public Order CalculateTotals(Order order)
        {
            if (order == null || !order.Items.Any())
            {
                order.UpdateTotals(0, 0, 0, 0, 0);
                return order;
            }

            // Calculate subtotal
            var subtotal = order.Items.Sum(item => item.LineTotal);

            // Calculate taxable amount (assuming all items are taxable)
            var taxableAmount = order.Items.Sum(item => item.LineTotal);

            // Calculate discount
            decimal discountAmount = 0;

            if (order.DiscountType == "Fixed" && order.DiscountValue.HasValue && order.DiscountValue.Value > 0)
            {
                discountAmount = Math.Min(order.DiscountValue.Value, subtotal);
            }
            else if (order.DiscountType == "Percentage" && order.DiscountValue.HasValue && order.DiscountValue.Value > 0)
            {
                var percentage = Math.Min(order.DiscountValue.Value, 100);
                discountAmount = subtotal * (percentage / 100);
            }

            // Calculate tax (proportional to taxable items)
            decimal taxAmount = 0;
            if (taxableAmount > 0 && subtotal > 0)
            {
                var discountProportion = taxableAmount / subtotal;
                var taxableDiscount = discountAmount * discountProportion;
                var discountedTaxableAmount = taxableAmount - taxableDiscount;
                taxAmount = discountedTaxableAmount * TAX_RATE;
            }

            // Calculate total
            var total = (subtotal - discountAmount) + taxAmount;

            order.UpdateTotals(subtotal, discountAmount, taxableAmount, taxAmount, total);

            return order;
        }
    }
}