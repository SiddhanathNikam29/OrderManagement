using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Interfaces
{
    public interface IReadRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}