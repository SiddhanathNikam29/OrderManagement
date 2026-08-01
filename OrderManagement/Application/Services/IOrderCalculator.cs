using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Services
{
    public interface IOrderCalculator
    {
        Order CalculateTotals(Order order);
    }
}