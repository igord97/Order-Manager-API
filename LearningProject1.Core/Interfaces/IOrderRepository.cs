using LearningProject1.Core.Models;

namespace LearningProject1.Core.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken ct);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<Order> AddAsync(Order order, CancellationToken ct);
}
